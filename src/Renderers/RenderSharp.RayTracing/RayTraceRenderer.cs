// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using ComputeSharp;
using RenderSharp.RayTracing.Models.BVH;
using RenderSharp.RayTracing.Models.Camera;
using RenderSharp.RayTracing.Models.Geometry;
using RenderSharp.RayTracing.Models.Lighting;
using RenderSharp.RayTracing.Setup;
using RenderSharp.RayTracing.Shaders.Pipeline;
using RenderSharp.RayTracing.Shaders.Pipeline.Collision;
using RenderSharp.RayTracing.Shaders.Pipeline.Collision.Enums;
using RenderSharp.RayTracing.Shaders.Pipeline.RayCasting;
using RenderSharp.RayTracing.Shaders.Pipeline.RayCasting.Camera;
using RenderSharp.RayTracing.Shaders.Shading;
using RenderSharp.RayTracing.Shaders.Shading.Interfaces;
using RenderSharp.RayTracing.Shaders.Shading.Stock.SkyShaders;
using RenderSharp.RayTracing.Utils;
using RenderSharp.Rendering.Analyzer.Enums;
using RenderSharp.Rendering.Analyzer.Interfaces;
using RenderSharp.Rendering.Interfaces;
using RenderSharp.Utilities.Tiles;
using System.Diagnostics.CodeAnalysis;
using CommonCamera = RenderSharp.Scenes.Cameras.Camera;
using CommonScene = RenderSharp.Scenes.Scene;

namespace RenderSharp.RayTracing;

/// Rendering Pipeline
///
///  Rendering Tile
///  |   Sample Loop
///  |   |
///  |   |  Init Sample
///  |   |  Cast Path Rays
///  |   |
///  |   |   Bounce Loop
///  |   |   |   Path Rays Collision
///  |   |   |   Cast Shadow Rays
///  |   |   |   Shadow Ray Collisions
///  |   |   |   Material Shaders
///  |   |   |   Sky Shader
///  |   |   Cache Sample Luminance
///  |   Write back

/// <summary>
/// An <see cref="IRenderer"/> implementation that uses ray tracing to render the scene.
/// </summary>
public class RayTracingRenderer : IRenderer
{
    private readonly List<MaterialShaderRunner> _shaderRunners;

    private BufferCollection? _buffers;
    private CommonCamera? _camera;
    private ReadOnlyBuffer<ObjectSpace>? _objectBuffer;
    private ReadOnlyBuffer<Vertex>? _vertexBuffer;
    private ReadOnlyBuffer<Triangle>? _geometryBuffer;
    private ReadOnlyBuffer<Light>? _lightBuffer;
    private ReadOnlyBuffer<BVHNode>? _bvhTreeBuffer;
    private int _bvhDepth;

    /// <summary>
    /// Initializes a new instance of the <see cref="RayTracingRenderer"/> class.
    /// </summary>
    public RayTracingRenderer()
    {
        Config = new RayTracingConfig();

        _shaderRunners = new List<MaterialShaderRunner>();
    }

    /// <summary>
    /// Gets or sets the ray trace renderer configurations.
    /// </summary>
    public RayTracingConfig Config { get; set; }
    
    /// <inheritdoc/>
    public GraphicsDevice? Device { get; set; }
    
    /// <inheritdoc/>
    public IReadWriteNormalizedTexture2D<float4>? RenderBuffer { get; set; }

    /// <inheritdoc/>
    public IRenderAnalyzer? RenderAnalyzer { get; set; }

    /// <inheritdoc/>
    public void SetupScene(CommonScene scene)
    {
        if (Device is null)
            return;

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

        if (Config.UseBVH)
        {
            // Build a BVH tree for geometry traversal
            RenderAnalyzer?.LogProcess("Build BVH tree", ProcessCategory.Setup);
            var bvhBuilder = loader.GetBVHBuilder();
            bvhBuilder.BuildBVHTree();

            // Store BVH heap and the heap depth
            _bvhTreeBuffer = bvhBuilder.BVHBuffer;
            _bvhDepth = bvhBuilder.Depth;
        }
    }

    /// <summary>
    /// Registers a material for use when rendering.
    /// </summary>
    /// <typeparam name="T">The shader type.</typeparam>
    /// <typeparam name="TMat">The material type</typeparam>
    /// <param name="material">The material properties.</param>
    public void RegisterMaterials<T, TMat>(TMat material)
        where T : struct, IMaterialShader<TMat>
        where TMat : struct
    {
        int id = _shaderRunners.Count;
        var runner = new MaterialShaderRunner<T, TMat>(id, material);
        _shaderRunners.Add(runner);
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

        // Prepare camera with aspect ratio
        var camera = new PinholeCamera(_camera.Transformation, _camera.Fov, imageRatio);

        // Allocate buffers if necessary
        // TODO: Preallocate largest tile buffer
        RenderAnalyzer?.LogProcess("Allocate Buffers", ProcessCategory.Rendering);
        _buffers ??= new BufferCollection(Device, tile, _objectBuffer, _vertexBuffer, _geometryBuffer, _bvhTreeBuffer, _lightBuffer, _bvhDepth);

        // Create Cast shaders
        CameraCastShaderRunner cameraCastRunner;
        if (Config.SampleCount == 1)
        {
            cameraCastRunner =
                new CameraCastShaderRunner<PinholeCameraCastShader>(
                    new PinholeCameraCastShader(tile, imageSize, camera, _buffers.PathRayBuffer));
        }
        else
        {
            cameraCastRunner =
                new CameraCastShaderRunner<ScatteredPinholeCameraCastShader>(
                    new ScatteredPinholeCameraCastShader(tile, imageSize, camera, _buffers.PathRayBuffer, _buffers.RandStateBuffer));
        }

        var shadowCastShader = new ShadowCastShader(_buffers.LightBuffer, _buffers.ShadowRayBuffer, _buffers.PathCastBuffer);
        
        var sampleCopyShader = new SampleCopyShader(tile, _buffers.LuminanceBuffer, RenderBuffer, Config.SampleCount);
        
        CollisionShaderRunner pathCollisionRunner;
        CollisionShaderRunner shadowIntersectRunner;

        // Collision Shaders
        if (Config.UseBVH)
        {
            pathCollisionRunner =
                new CollisionShaderRunner<GeometryCollisionBVHTreeShader>(
                    new GeometryCollisionBVHTreeShader(
                        _buffers.VertexBuffer, _buffers.GeometryBuffer,
                        _buffers.BVHTreeBuffer, _buffers.PathRayBuffer,
                        _buffers.PathCastBuffer, _buffers.BVHStackBuffer,
                        _bvhDepth, (int)CollisionMode.Nearest));
            
            shadowIntersectRunner =
                new CollisionShaderRunner<GeometryCollisionBVHTreeShader>(
                    new GeometryCollisionBVHTreeShader(
                        _buffers.VertexBuffer, _buffers.GeometryBuffer,
                        _buffers.BVHTreeBuffer, _buffers.ShadowRayBuffer,
                        _buffers.ShadowCastBuffer, _buffers.BVHStackBuffer,
                        _bvhDepth, (int)CollisionMode.Any));
        }
        else
        {
            pathCollisionRunner =
                new CollisionShaderRunner<GeometryCollisionShader>(
                    new GeometryCollisionShader(
                        _buffers.VertexBuffer, _buffers.GeometryBuffer,
                        _buffers.PathRayBuffer, _buffers.PathCastBuffer,
                        (int)CollisionMode.Nearest));
            
            shadowIntersectRunner =
                new CollisionShaderRunner<GeometryCollisionShader>(
                    new GeometryCollisionShader(
                        _buffers.VertexBuffer, _buffers.GeometryBuffer,
                        _buffers.ShadowRayBuffer, _buffers.ShadowCastBuffer,
                        (int)CollisionMode.Any));
        }

        // Create sky shader
        // TODO: Load externally
        var skyShader = new SolidSkyShader(new float4(0.25f, 0.35f, 0.5f, 1f), _buffers.PathRayBuffer, _buffers.PathCastBuffer, _buffers.AttenuationBuffer, _buffers.LuminanceBuffer);

        RenderAnalyzer?.LogProcess("Render Loop", ProcessCategory.Rendering);
        using var context = Device.CreateComputeContext();

        // Allow unreachable code (for debug inserts)
        #pragma warning disable CS0162

        // Sample loop
        for (int s = 0; s < Config.SampleCount; s++)
        {
            var initShader = new SampleInitializeShader(_buffers.AttenuationBuffer, _buffers.LuminanceBuffer, _buffers.RandStateBuffer, s);

            // Initialize the buffers
            context.For(tile.Width, tile.Height, initShader);

            // Create the rays from the camera
            cameraCastRunner.Enqueue(context, tile.Width, tile.Height);
            context.Barrier(_buffers.PathRayBuffer);

            // Bounce Loop
            for (int b = 0; b < Config.MaxBounceDepth; b++)
            {
                // Find object collision and cache the resulting ray cast
                pathCollisionRunner.Enqueue(context, tile.Width * tile.Height);
                context.Barrier(_buffers.PathCastBuffer);

                // Create shadow ray casts
                context.For(tile.Width, tile.Height, _lightBuffer.Length, shadowCastShader);
                context.Barrier(_buffers.ShadowRayBuffer);

                // Detect shadow ray collisions
                shadowIntersectRunner.Enqueue(context, tile.Width * tile.Height * _lightBuffer.Length);
                context.Barrier(_buffers.ShadowRayBuffer);

                // Apply material shaders
                foreach (var runner in _shaderRunners)
                {
                    runner.SetShaderBuffers(_buffers);
                    runner.Enqueue(in context, tile);
                }
                context.Barrier(_buffers.AttenuationBuffer);
                context.Barrier(_buffers.LuminanceBuffer);

                // Apply sky material
                context.For(tile.Width, tile.Height, skyShader);
                context.Barrier(_buffers.AttenuationBuffer);
                context.Barrier(_buffers.LuminanceBuffer);
            }

            // Copy color buffer to color sum buffer
            context.For(tile.Width, tile.Height, sampleCopyShader);
            context.Barrier(RenderBuffer);
        }
    }

    [MemberNotNull(
        nameof(Device),
        nameof(RenderBuffer),
        nameof(_objectBuffer),
        nameof(_vertexBuffer),
        nameof(_geometryBuffer),
        nameof(_lightBuffer),
        nameof(_camera))]
    private void GuardReady()
    {
        Guard.IsNotNull(Device);
        Guard.IsNotNull(RenderBuffer);
        Guard.IsNotNull(_objectBuffer);
        Guard.IsNotNull(_vertexBuffer);
        Guard.IsNotNull(_geometryBuffer);
        Guard.IsNotNull(_lightBuffer);
        Guard.IsNotNull(_camera);
    }
}
