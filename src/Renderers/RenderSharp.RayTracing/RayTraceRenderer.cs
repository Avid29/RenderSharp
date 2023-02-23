// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using ComputeSharp;
using RenderSharp.RayTracing.Models;
using RenderSharp.RayTracing.Models.BVH;
using RenderSharp.RayTracing.Models.Camera;
using RenderSharp.RayTracing.Models.Geometry;
using RenderSharp.RayTracing.Models.Lighting;
using RenderSharp.RayTracing.Models.Materials;
using RenderSharp.RayTracing.Models.Rays;
using RenderSharp.RayTracing.Setup;
using RenderSharp.RayTracing.Shaders.Debugging;
using RenderSharp.RayTracing.Shaders.Debugging.Enums;
using RenderSharp.RayTracing.Shaders.Rendering;
using RenderSharp.RayTracing.Shaders.Shading.Stock.MaterialShaders;
using RenderSharp.RayTracing.Shaders.Shading.Stock.SkyShaders;
using RenderSharp.Rendering.Enums;
using RenderSharp.Rendering.Interfaces;
using RenderSharp.Scenes.Geometry;
using RenderSharp.Utilities.Tiles;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using CommonCamera = RenderSharp.Scenes.Cameras.Camera;
using CommonScene = RenderSharp.Scenes.Scene;

namespace RenderSharp.RayTracing;

/// <summary>
/// An <see cref="IRenderer"/> implementation that uses ray tracing to render the scene.
/// </summary>
public class RayTracingRenderer : IRenderer
{
    private CommonCamera? _camera;
    private ReadOnlyBuffer<Vertex>? _vertexBuffer;
    private ReadOnlyBuffer<Triangle>? _geometryBuffer;
    private ReadOnlyBuffer<Light>? _lightsBuffer;
    private ReadOnlyBuffer<BVHNode>? _bvhBuffer;
    private int _objectCount;
    private int _bvhDepth;

    /// <summary>
    /// Initializes a new instance of the <see cref="RayTracingRenderer"/> class.
    /// </summary>
    public RayTracingRenderer(GraphicsDevice device)
    {
        Device = device;
    }
    
    /// <inheritdoc/>
    public GraphicsDevice Device { get; }
    
    /// <inheritdoc/>
    public IReadWriteNormalizedTexture2D<float4>? RenderBuffer { get; set; }

    /// <inheritdoc/>
    public IRenderAnalyzer? RenderAnalyzer { get; set; }

    /// <inheritdoc/>
    public void SetupScene(CommonScene scene)
    {
        RenderAnalyzer?.LogProcess("Load Objects", ProcessCategory.Setup);
        _camera = scene.ActiveCamera;

        // Load geometry objects to the geometry buffer
        var loader = new ObjectLoader(Device);
        loader.LoadScene(scene);

        // Store geometry and object count
        _vertexBuffer = loader.VertexBuffer;
        _geometryBuffer = loader.GeometryBuffer;
        _lightsBuffer = loader.LightsBuffer;
        _objectCount = loader.ObjectCount;

        // Build a BVH tree for geometry traversal
        RenderAnalyzer?.LogProcess("Build BVH tree", ProcessCategory.Setup);
        var bvhBuilder = loader.GetBVHBuilder();
        bvhBuilder.BuildBVHTree();

        // Store BVH heap and the heap depth
        _bvhBuffer = bvhBuilder.BVHBuffer;
        _bvhDepth = bvhBuilder.Depth;
    }

    /// <inheritdoc/>
    public void Render()
    {
        Guard.IsNotNull(RenderBuffer);

        var size = new int2(RenderBuffer.Width, RenderBuffer.Height);
        var tile = new Tile(int2.Zero, size);
        RenderSegment(tile);
    }

    /// <inheritdoc/>
    public void RenderSegment(Tile tile)
    {
        GuardReady();

        int imageWidth = RenderBuffer.Width;
        int imageHeight = RenderBuffer.Height;
        float imageRatio = (float)imageWidth / imageHeight;
        var imageSize = new int2(imageWidth, imageHeight);
        int tilePixelCount = tile.Width * tile.Height;
        int samplesSqrt = 1;
        int samples = samplesSqrt * samplesSqrt;

        // Prepare camera with aspect ratio
        var camera = new Camera(_camera.Transformation, _camera.Fov, imageRatio);

        // Allocate buffers
        RenderAnalyzer?.LogProcess("Allocate Buffers", ProcessCategory.Rendering);
        ReadWriteBuffer<Rand> randBuffer = Device.AllocateReadWriteBuffer<Rand>(tilePixelCount);
        ReadWriteTexture3D<int> bvhStack = Device.AllocateReadWriteTexture3D<int>(tile.Width, tile.Height, _bvhDepth + 1);
        ReadWriteBuffer<Ray> rayBuffer = Device.AllocateReadWriteBuffer<Ray>(tilePixelCount);
        ReadWriteBuffer<Ray> shadowRayBuffer = Device.AllocateReadWriteBuffer<Ray>(tilePixelCount * _lightsBuffer.Length);
        ReadWriteBuffer<GeometryCollision> rayCastBuffer = Device.AllocateReadWriteBuffer<GeometryCollision>(tilePixelCount);
        IReadWriteNormalizedTexture2D<float4> colorSumBuffer = Device.AllocateReadWriteTexture2D<Rgba32, float4>(tile.Width, tile.Height);
        IReadWriteNormalizedTexture2D<float4> colorBuffer = Device.AllocateReadWriteTexture2D<Rgba32, float4>(tile.Width, tile.Height);
        IReadWriteNormalizedTexture2D<float4> attenuationBuffer = Device.AllocateReadWriteTexture2D<Rgba32, float4>(tile.Width, tile.Height);


        var color = new Vector3(0.2f, 0.5f, 0.8f);
        var material = new PhongMaterial(color, Vector3.One, color, 80f,
                            cDiffuse: 0.8f, cSpecular: 0.9f, cAmbient: 0.2f);

        // Create shaders
        var cameraShader = new CameraCastShader(tile, imageSize, camera, rayBuffer);
        var collisionShader = new GeometryCollisionShader(_vertexBuffer, _geometryBuffer, rayBuffer, rayCastBuffer);
        //var collisionShader = new GeometryCollisionBVHTreeShader(tile, bvhStack, _bvhBuffer, _geometryBuffer, rayBuffer, rayCastBuffer);
        var shadowCastShader = new ShadowCastShader(_lightsBuffer, shadowRayBuffer, rayCastBuffer);
        var shadowIntersectShader = new ShadowIntersectionShader(_vertexBuffer, _geometryBuffer, shadowRayBuffer);
        var materialShader = new PhongShader(0, material, _lightsBuffer, rayBuffer, shadowRayBuffer, rayCastBuffer, colorBuffer);
        var skyShader = new SolidSkyShader(new float4(0.25f, 0.35f, 0.5f, 1f), rayBuffer, rayCastBuffer, attenuationBuffer, colorBuffer);
        var sampleCopyShader = new SampleCopyShader(colorBuffer, colorSumBuffer);
        var tileCopyShader = new TileCopyShader(tile, colorSumBuffer, RenderBuffer, samples);

        RenderAnalyzer?.LogProcess("Render Loop", ProcessCategory.Rendering);
        using var context = Device.CreateComputeContext();

        #pragma warning disable

        context.Fill(colorSumBuffer, float4.Zero);

        for (int s = 0; s < samples; s++)
        {
            var initShader = new SampleInitializeShader(attenuationBuffer, colorBuffer, randBuffer, s);
            //var cameraShader = new ScatteredCameraCastShader(tile, imageSize, camera, rayBuffer, randBuffer, s, samplesSqrt);

            // Initialize the buffers
            context.For(tile.Width, tile.Height, initShader);

            // Create the rays from the camera
            context.For(tile.Width, tile.Height, cameraShader);
            context.Barrier(rayBuffer);

            // Bounces
            for (int b = 0; b < 1; b++)
            {
                // Find object collision and cache the resulting ray cast 
                context.For(tile.Width, tile.Height, collisionShader);
                context.Barrier(rayCastBuffer);

                //context.For(tile.Width, tile.Height, new RayCastBufferDumpShader(tile, rayCastBuffer, _geometryBuffer, RenderBuffer, _objectCount, (int)RayCastDumpValueType.Object));
                //return;

                // Create shadow ray casts
                context.For(tile.Width, tile.Height, _lightsBuffer.Length, shadowCastShader);
                context.Barrier(shadowRayBuffer);

                // Detect shadow ray collisions
                context.For(tile.Width, tile.Height, _lightsBuffer.Length, shadowIntersectShader);
                context.Barrier(shadowRayBuffer);

                // Apply object materials
                context.For(tile.Width, tile.Height, materialShader);
                context.Barrier(attenuationBuffer);
                context.Barrier(colorBuffer);

                // Apply sky material
                context.For(tile.Width, tile.Height, skyShader);
                context.Barrier(attenuationBuffer);
                context.Barrier(colorBuffer);
            }

            // Copy color buffer to color sum buffer
            context.For(tile.Width, tile.Height, sampleCopyShader);
            context.Barrier(colorSumBuffer);
        }

        context.For(tile.Width, tile.Height, tileCopyShader);
        context.Barrier(RenderBuffer);

        // Dump the ray cast's directions to the render buffer (for debugging)
        //context.For(width, height, new RayCastBufferDumpShader(rayCastBuffer, _geometryBuffer, RenderBuffer, _objectCount, (int)RayCastDumpValueType.Distance));
    }

    [MemberNotNull(
        nameof(RenderBuffer),
        nameof(_geometryBuffer),
        nameof(_lightsBuffer),
        nameof(_camera))]
    private void GuardReady()
    {
        Guard.IsNotNull(_camera);
        Guard.IsNotNull(RenderBuffer);
        Guard.IsNotNull(_vertexBuffer);
        Guard.IsNotNull(_geometryBuffer);
        Guard.IsNotNull(_lightsBuffer);
    }
}
