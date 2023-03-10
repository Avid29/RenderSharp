// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using ComputeSharp;
using RenderSharp.RayTracing.Models.BVH;
using RenderSharp.RayTracing.Models.Camera;
using RenderSharp.RayTracing.Models.Geometry;
using RenderSharp.RayTracing.Models.Lighting;
using RenderSharp.RayTracing.Models.Materials;
using RenderSharp.RayTracing.Models.Materials.Enums;
using RenderSharp.RayTracing.Setup;
using RenderSharp.RayTracing.Shaders.Rendering;
using RenderSharp.RayTracing.Shaders.Shading;
using RenderSharp.RayTracing.Shaders.Shading.Stock.MaterialShaders;
using RenderSharp.RayTracing.Shaders.Shading.Stock.SkyShaders;
using RenderSharp.Rendering.Enums;
using RenderSharp.Rendering.Interfaces;
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
    private ReadOnlyBuffer<ObjectSpace>? _objectBuffer;
    private ReadOnlyBuffer<Vertex>? _vertexBuffer;
    private ReadOnlyBuffer<Triangle>? _geometryBuffer;
    private ReadOnlyBuffer<Light>? _lightBuffer;
    private ReadOnlyBuffer<BVHNode>? _bvhBuffer;
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
        _objectBuffer = loader.ObjectBuffer;
        _vertexBuffer = loader.VertexBuffer;
        _geometryBuffer = loader.GeometryBuffer;
        _lightBuffer = loader.LightsBuffer;

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
        int samplesSqrt = 1;
        int samples = samplesSqrt * samplesSqrt;

        // Prepare camera with aspect ratio
        var camera = new Camera(_camera.Transformation, _camera.Fov, imageRatio);

        // Allocate buffers
        RenderAnalyzer?.LogProcess("Allocate Buffers", ProcessCategory.Rendering);

        var bc = new TileBufferCollection(Device, tile, _objectBuffer, _vertexBuffer, _geometryBuffer, _lightBuffer);

        // Create shaders
        var cameraShader = new CameraCastShader(tile, imageSize, camera, bc.RayBuffer);
        var collisionShader = new GeometryCollisionShader(bc.VertexBuffer, bc.GeometryBuffer, bc.RayBuffer, bc.RayCastBuffer);
        //var collisionShader = new GeometryCollisionBVHTreeShader(bvhStack, _bvhBuffer, _vertexBuffer _geometryBuffer, rayBuffer, rayCastBuffer);
        var shadowCastShader = new ShadowCastShader(bc.LightBuffer, bc.ShadowCastBuffer, bc.RayCastBuffer);
        var shadowIntersectShader = new ShadowIntersectionShader(bc.VertexBuffer, bc.GeometryBuffer, bc.ShadowCastBuffer);
        var skyShader = new SolidSkyShader(new float4(0.25f, 0.35f, 0.5f, 1f), bc.RayBuffer, bc.RayCastBuffer, bc.AttenuationBuffer, bc.ColorBuffer);
        var sampleCopyShader = new SampleCopyShader(tile, bc.ColorBuffer, RenderBuffer, samples);

        // Create materials
        // TODO: Load dynamically
        var color0 = new Vector3(0.9f, 0.2f, 0.1f);
        var material0 = new PhongMaterial(color0, Vector3.One, color0, 80f,
            cDiffuse: 0.8f, cSpecular: 0.9f, cAmbient: 0.2f);

        var color1 = new Vector3(0.2f, 0.5f, 0.8f);
        var material1 = new PhongMaterial(color1, Vector3.One, color1, 80f,
            cDiffuse: 0.8f, cSpecular: 0.9f, cAmbient: 0.2f);

        var yellow = Vector3.UnitX + Vector3.UnitY;
        var material2 = new CheckeredPhongMaterial(yellow, Vector3.UnitX, Vector3.One, 80f, 10f,
            cDiffuse0: 0.8f, cDiffuse1: 0.8f, cSpecular: 0.9f, cAmbient: 0.2f);

        var material3 = new RadialGradientPhongMaterial(Vector3.UnitX, Vector3.UnitY, Vector3.One, 80f, 4f, (int)TextureSpace.Object,
            cDiffuse0: 0.8f, cDiffuse1: 0.8f, cSpecular: 0.9f, cAmbient: 0.2f);

        var materialShadersRunners = new MaterialShaderRunner[]
        {
            new MaterialShaderRunner<PhongShader>(new PhongShader(2, material0), bc),
            //new MaterialShaderRunner<PhongShader>(new PhongShader(1, material1), bc),
            //new MaterialShaderRunner<CheckeredPhongShader>(new CheckeredPhongShader(0, material2), bc),
            //new MaterialShaderRunner<RadialGradientPhongShader>(new RadialGradientPhongShader(0, material3), bc),
            new MaterialShaderRunner<VoronoiPhongShader>(new VoronoiPhongShader(0), bc),
        };

        RenderAnalyzer?.LogProcess("Render Loop", ProcessCategory.Rendering);
        using var context = Device.CreateComputeContext();

        #pragma warning disable

        for (int s = 0; s < samples; s++)
        {
            var initShader = new SampleInitializeShader(bc.AttenuationBuffer, bc.ColorBuffer, bc.RandBuffer, s);
            //var cameraShader = new ScatteredCameraCastShader(tile, imageSize, camera, rayBuffer, randBuffer, s, samplesSqrt);

            // Initialize the buffers
            context.For(tile.Width, tile.Height, initShader);

            // Create the rays from the camera
            context.For(tile.Width, tile.Height, cameraShader);
            context.Barrier(bc.RayBuffer);


            // Bounces
            for (int b = 0; b < 1; b++)
            {
                // Find object collision and cache the resulting ray cast 
                context.For(tile.Width, tile.Height, collisionShader);
                context.Barrier(bc.RayCastBuffer);

                // Create shadow ray casts
                context.For(tile.Width, tile.Height, _lightBuffer.Length, shadowCastShader);
                context.Barrier(bc.ShadowCastBuffer);

                // Detect shadow ray collisions
                context.For(tile.Width, tile.Height, _lightBuffer.Length, shadowIntersectShader);
                context.Barrier(bc.ShadowCastBuffer);

                // Apply material shaders
                foreach (var runner in materialShadersRunners)
                    runner.Enqueue(in context, tile);
                context.Barrier(bc.AttenuationBuffer);
                context.Barrier(bc.ColorBuffer);

                // Apply sky material
                context.For(tile.Width, tile.Height, skyShader);
                context.Barrier(bc.AttenuationBuffer);
                context.Barrier(bc.ColorBuffer);
            }

            // Copy color buffer to color sum buffer
            context.For(tile.Width, tile.Height, sampleCopyShader);
            context.Barrier(RenderBuffer);
        }
    }

    [MemberNotNull(
        nameof(RenderBuffer),
        nameof(_objectBuffer),
        nameof(_vertexBuffer),
        nameof(_geometryBuffer),
        nameof(_lightBuffer),
        nameof(_camera))]
    private void GuardReady()
    {
        Guard.IsNotNull(RenderBuffer);
        Guard.IsNotNull(_objectBuffer);
        Guard.IsNotNull(_vertexBuffer);
        Guard.IsNotNull(_geometryBuffer);
        Guard.IsNotNull(_lightBuffer);
        Guard.IsNotNull(_camera);
    }
}
