// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using ComputeSharp;
using RenderSharp.RayTracing.Conversion;
using RenderSharp.RayTracing.Scene.Camera;
using RenderSharp.RayTracing.Scene.Geometry;
using RenderSharp.RayTracing.Scene.Rays;
using RenderSharp.RayTracing.Shaders.Debugging;
using RenderSharp.RayTracing.Shaders.Debugging.Enums;
using RenderSharp.RayTracing.Shaders.Rendering;
using RenderSharp.RayTracing.Shaders.Shading.Stock.MaterialShaders;
using RenderSharp.RayTracing.Shaders.Shading.Stock.SkyShaders;
using RenderSharp.Rendering;
using RenderSharp.Scenes.Geometry;
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
    private int _objectCount;

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

        _geometryBuffer = loader.GeometryBuffer;
        _objectCount = loader.ObjectCount;
    }

    /// <inheritdoc/>
    public void Render()
    {
        GuardReady();

        int width = RenderBuffer.Width;
        int height = RenderBuffer.Height;
        float ratio = (float)width / height;
        int2 size = new(width, height);
        int pixelCount = width * height;

        // Prepare camera with aspect ratio
        var camera = new Camera(_camera.Transformation, _camera.Fov, ratio);

        // Allocate buffers
        ReadWriteBuffer<Ray> rayBuffer = Device.AllocateReadWriteBuffer<Ray>(pixelCount);
        ReadWriteBuffer<RayCast> rayCastBuffer = Device.AllocateReadWriteBuffer<RayCast>(pixelCount);
        IReadWriteNormalizedTexture2D<float4> attenuationBuffer = Device.AllocateReadWriteTexture2D<Rgba32, float4>(width, height);

        using var context = Device.CreateComputeContext();

        // Initialize the attenuation buffer
        context.Fill(RenderBuffer, float4.Zero);
        context.Fill(attenuationBuffer, float4.One);

        // Create the rays from the camera
        context.For(width, height, new CameraCastShader(size, camera, rayBuffer));
        context.Barrier(rayBuffer);

        for (int i = 0; i < 4; i++)
        {
            // Find object collision and cache the resulting ray cast 
            context.For(width, height, new GeometryCollisionShader(_geometryBuffer, rayBuffer, rayCastBuffer));
            context.Barrier(rayCastBuffer);

            var material = new GlossyMaterial
            {
                albedo = 0.9f * Vector4.One,
                roughness = 0.8f,
            };
            context.For(width, height, new GlossyShader(0, material, rayBuffer, rayCastBuffer, attenuationBuffer, RenderBuffer));
            context.Barrier(attenuationBuffer);
            context.Barrier(RenderBuffer);

            // Calculate the color of the sky
            context.For(width, height, new SolidSkyShader(new float4(0.5f, 0.7f, 1f, 1f), rayBuffer, rayCastBuffer, attenuationBuffer, RenderBuffer));
            context.Barrier(RenderBuffer);

            //context.For(width, height, new RayCastBufferDumpShader(rayCastBuffer, _geometryBuffer, RenderBuffer, _objectCount, (int)RayCastDumpValueType.Object));
        }

        // Dump the ray cast's directions to the render buffer (for debugging)
        //context.For(width, height, new RayCastBufferDumpShader(rayCastBuffer, _geometryBuffer, RenderBuffer, _objectCount, (int)RayCastDumpValueType.Distance));
    }

    /// <inheritdoc/>
    public void RenderSegment(int2 offset, int2 size)
    {
        Guard.IsNotNull(RenderBuffer);

        // TODO: Support rendering the scene segments at a time.

        throw new NotImplementedException();
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
