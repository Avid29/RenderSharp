// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using ComputeSharp;
using RenderSharp.RayTracing.Scene.BVH;
using RenderSharp.RayTracing.Scene.Camera;
using RenderSharp.RayTracing.Scene.Geometry;
using RenderSharp.RayTracing.Scene.Rays;
using RenderSharp.RayTracing.Setup;
using RenderSharp.RayTracing.Shaders.Rendering;
using RenderSharp.RayTracing.Shaders.Shading.Stock.MaterialShaders;
using RenderSharp.RayTracing.Shaders.Shading.Stock.SkyShaders;
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
    private ReadOnlyBuffer<Triangle>? _geometryBuffer;
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
    public void SetupScene(CommonScene scene)
    {
        _camera = scene.ActiveCamera;

        // Load geometry objects to the geometry buffer
        var loader = new ObjectLoader(Device);
        var geometryObjects = scene.Objects
            .OfType<GeometryObject>().ToList();
        loader.LoadObjects(geometryObjects);

        // Store geometry and object count
        _geometryBuffer = loader.GeometryBuffer;
        _objectCount = loader.ObjectCount;

        // Build a BVH tree for geometry traversal
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

        // Prepare camera with aspect ratio
        var camera = new Camera(_camera.Transformation, _camera.Fov, imageRatio);

        // Allocate buffers
        ReadWriteTexture3D<int> bvhStack = Device.AllocateReadWriteTexture3D<int>(tile.Width, tile.Height, _bvhDepth + 1);
        ReadWriteBuffer<Ray> rayBuffer = Device.AllocateReadWriteBuffer<Ray>(tilePixelCount);
        ReadWriteBuffer<RayCast> rayCastBuffer = Device.AllocateReadWriteBuffer<RayCast>(tilePixelCount);
        IReadWriteNormalizedTexture2D<float4> attenuationBuffer = Device.AllocateReadWriteTexture2D<Rgba32, float4>(tile.Width, tile.Height);

        // Create glossy material
        var material = new GlossyMaterial
        {
            albedo = 0.9f * Vector4.One,
            roughness = 0.8f,
        };

        // Create shaders
        var cameraShader = new CameraCastShader(tile, imageSize, camera, rayBuffer);
        //var collisionShader = new GeometryCollisionShader(tile, _geometryBuffer, rayBuffer, rayCastBuffer);
        var collisionShader = new GeometryCollisionBVHTreeShader(tile, bvhStack, _bvhBuffer, _geometryBuffer, rayBuffer, rayCastBuffer);
        var materialShader = new GlossyShader(tile, 0, material, rayBuffer, rayCastBuffer, attenuationBuffer, RenderBuffer);
        var skyShader = new SolidSkyShader(tile, new float4(0.5f, 0.7f, 1f, 1f), rayBuffer, rayCastBuffer, attenuationBuffer, RenderBuffer);

        using var context = Device.CreateComputeContext();

        // Initialize the attenuation buffer
        //context.Fill(RenderBuffer, float4.Zero);
        context.Fill(attenuationBuffer, float4.One);

        // Create the rays from the camera
        context.For(tile.Width, tile.Height, cameraShader);
        context.Barrier(rayBuffer);

        for (int i = 0; i < 4; i++)
        {
            // Find object collision and cache the resulting ray cast 
            context.For(tile.Width, tile.Height, collisionShader);
            context.Barrier(rayCastBuffer);

            context.For(tile.Width, tile.Height, materialShader);
            context.Barrier(attenuationBuffer);
            context.Barrier(RenderBuffer);

            // Calculate the color of the sky
            context.For(tile.Width, tile.Height, skyShader);
            context.Barrier(RenderBuffer);

            //context.For(width, height, new RayCastBufferDumpShader(rayCastBuffer, _geometryBuffer, RenderBuffer, _objectCount, (int)RayCastDumpValueType.Object));
        }

        // Dump the ray cast's directions to the render buffer (for debugging)
        //context.For(width, height, new RayCastBufferDumpShader(rayCastBuffer, _geometryBuffer, RenderBuffer, _objectCount, (int)RayCastDumpValueType.Distance));
    }

    [MemberNotNull(
        nameof(RenderBuffer),
        nameof(_geometryBuffer),
        nameof(_camera))]
    private void GuardReady()
    {
        Guard.IsNotNull(_camera);
        Guard.IsNotNull(RenderBuffer);
        Guard.IsNotNull(_geometryBuffer);
    }
}
