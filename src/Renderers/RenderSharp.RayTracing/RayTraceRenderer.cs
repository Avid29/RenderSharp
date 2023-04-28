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
using RenderSharp.RayTracing.Shaders.Pipeline;
using RenderSharp.RayTracing.Shaders.Pipeline.CameraCasting;
using RenderSharp.RayTracing.Shaders.Pipeline.Collision;
using RenderSharp.RayTracing.Shaders.Pipeline.Collision.Enums;
using RenderSharp.RayTracing.Shaders.Shading;
using RenderSharp.RayTracing.Shaders.Shading.Stock.MaterialShaders;
using RenderSharp.RayTracing.Shaders.Shading.Stock.SkyShaders;
using RenderSharp.RayTracing.Utils;
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
    private ReadOnlyBuffer<BVHNode>? _bvhTreeBuffer;
    private int _bvhDepth;

    /// <summary>
    /// Initializes a new instance of the <see cref="RayTracingRenderer"/> class.
    /// </summary>
    public RayTracingRenderer()
    {
    }
    
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
        _camera = scene.ActiveActiveCamera;

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
        _bvhTreeBuffer = bvhBuilder.BVHBuffer;
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
        var camera = new PinholeCamera(_camera.Transformation, _camera.Fov, imageRatio);

        // Allocate buffers
        RenderAnalyzer?.LogProcess("Allocate Buffers", ProcessCategory.Rendering);

        var bc = new TileBufferCollection(Device, tile, _objectBuffer, _vertexBuffer, _geometryBuffer, _bvhTreeBuffer, _lightBuffer, _bvhDepth);

        // Create shaders
        // Cast shaders
        var cameraCastShader = new CameraCastShader(tile, imageSize, camera, bc.PathRayBuffer);
        var shadowCastShader = new ShadowCastShader(bc.LightBuffer, bc.ShadowRayBuffer, bc.PathCastBuffer);
        // Collision Shaders
        var collisionShader = new GeometryCollisionShader(bc.VertexBuffer, bc.GeometryBuffer, bc.PathRayBuffer, bc.PathCastBuffer, (int)CollisionMode.Nearest);
        //var collisionShader = new GeometryCollisionBVHTreeShader(bc.VertexBuffer, bc.GeometryBuffer, bc.BVHTreeBuffer, bc.PathRayBuffer, bc.PathCastBuffer, bc.BVHStackBuffer, _bvhDepth, (int)CollisionMode.Nearest);
        var shadowIntersectShader = new GeometryCollisionShader(bc.VertexBuffer, bc.GeometryBuffer, bc.ShadowRayBuffer, bc.ShadowCastBuffer, (int)CollisionMode.Any);

        var skyShader = new SolidSkyShader(new float4(0.25f, 0.35f, 0.5f, 1f), bc.PathRayBuffer, bc.PathCastBuffer, bc.AttenuationBuffer, bc.LuminanceBuffer);
        var sampleCopyShader = new SampleCopyShader(tile, bc.LuminanceBuffer, RenderBuffer, samples);

        // Create materials
        // TODO: Load dynamically
        var color0 = new Vector3(0.9f, 0.2f, 0.1f);
        var material0 = new PhongMaterial(color0, Vector3.One, color0, 80f,
            cDiffuse: 0.8f, cSpecular: 0.9f, cAmbient: 0.2f);

        var color1 = new Vector3(0.25f, 0.35f, 0.35f);
        var material1 = new PhongMaterial(color1, Vector3.One, color1, 10f,
            cDiffuse: 0.8f, cSpecular: 0.9f, cAmbient: 0.2f);

        var yellow = Vector3.UnitX + Vector3.UnitY;
        var material2 = new CheckeredPhongMaterial(yellow, Vector3.UnitX, Vector3.One, 50f, 10f,
            cDiffuse0: 0.8f, cDiffuse1: 0.8f, cSpecular: 0.9f, cAmbient: 0.2f);

        var material3 = new RadialGradientPhongMaterial(Vector3.UnitX, Vector3.UnitY, Vector3.One, 50f, 4f, (int)TextureSpace.Object,
            cDiffuse0: 0.8f, cDiffuse1: 0.8f, cSpecular: 0.9f, cAmbient: 0.2f);

        var material4 = new PrincipledMaterial(Vector3.One * 0.5f, Vector3.One * 0.5f, Vector3.Zero, 10f, 0.8f, 0, 1);
        var material5 = new PrincipledMaterial(new Vector3(0.25f, 0.35f, 0.35f), Vector3.One * 0.5f, Vector3.Zero, 20f, 0.025f, 0, 1);
        
        var material6 = new PrincipledMaterial(Vector3.One * 0.5f, Vector3.One, Vector3.Zero, 20f, 0f, 0.9f, 0.95f);
;
        var materialShadersRunners = new MaterialShaderRunner[]
        {
            new MaterialShaderRunner<PrincipledShader>(new PrincipledShader(1, material4), bc),
            //new MaterialShaderRunner<PrincipledShader>(new PrincipledShader(1, material5), bc),
            new MaterialShaderRunner<PrincipledShader>(new PrincipledShader(2, material6), bc),
            //new MaterialShaderRunner<GlossyShader>(new GlossyShader(2), bc),
            //new MaterialShaderRunner<PhongShader>(new PhongShader(2, material0), bc),
            //new MaterialShaderRunner<PhongShader>(new PhongShader(1, material1), bc),
            new MaterialShaderRunner<CheckeredPhongShader>(new CheckeredPhongShader(0, material2), bc),
            //new MaterialShaderRunner<RadialGradientPhongShader>(new RadialGradientPhongShader(0, material3), bc),
        };

        RenderAnalyzer?.LogProcess("Render Loop", ProcessCategory.Rendering);
        using var context = Device.CreateComputeContext();

        #pragma warning disable

        for (int s = 0; s < samples; s++)
        {
            var initShader = new SampleInitializeShader(bc.AttenuationBuffer, bc.LuminanceBuffer, bc.RandStateBuffer, s);
            //var cameraShader = new ScatteredCameraCastShader(tile, imageSize, camera, rayBuffer, randBuffer, s, samplesSqrt);

            // Initialize the buffers
            context.For(tile.Width, tile.Height, initShader);

            // Create the rays from the camera
            context.For(tile.Width, tile.Height, cameraCastShader);
            context.Barrier(bc.PathRayBuffer);

            // Bounces
            for (int b = 0; b < 8; b++)
            {
                // Find object collision and cache the resulting ray cast 
                context.For(tile.Width * tile.Height, collisionShader);
                context.Barrier(bc.PathCastBuffer);

                //// DEBUG: print object collisions
                //context.For(tile.Width, tile.Height, new GeometryCollisionBufferDumpShader(tile, bc.PathCastBuffer, bc.GeometryBuffer, bc.ObjectBuffer, RenderBuffer, (int)GeometryCollisionDumpValueType.Object));
                //return;

                // Create shadow ray casts
                context.For(tile.Width, tile.Height, _lightBuffer.Length, shadowCastShader);
                context.Barrier(bc.ShadowRayBuffer);

                // Detect shadow ray collisions
                context.For(tile.Width * tile.Height * _lightBuffer.Length, shadowIntersectShader);
                context.Barrier(bc.ShadowRayBuffer);

                // Apply material shaders
                foreach (var runner in materialShadersRunners)
                    runner.Enqueue(in context, tile);
                context.Barrier(bc.AttenuationBuffer);
                context.Barrier(bc.LuminanceBuffer);

                // Apply sky material
                context.For(tile.Width, tile.Height, skyShader);
                context.Barrier(bc.AttenuationBuffer);
                context.Barrier(bc.LuminanceBuffer);
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
        nameof(_bvhTreeBuffer),
        nameof(_lightBuffer),
        nameof(_camera))]
    private void GuardReady()
    {
        Guard.IsNotNull(RenderBuffer);
        Guard.IsNotNull(_objectBuffer);
        Guard.IsNotNull(_vertexBuffer);
        Guard.IsNotNull(_geometryBuffer);
        Guard.IsNotNull(_bvhTreeBuffer);
        Guard.IsNotNull(_lightBuffer);
        Guard.IsNotNull(_camera);
    }
}
