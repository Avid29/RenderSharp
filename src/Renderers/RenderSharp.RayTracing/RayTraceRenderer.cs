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
using RenderSharp.Rendering;
using RenderSharp.Scenes.Geometry;
using System.Diagnostics.CodeAnalysis;

using CommonCamera = RenderSharp.Scenes.Cameras.Camera;
using CommonScene = RenderSharp.Scenes.Scene;

namespace RenderSharp.RayTracing;

public class RayTracingRenderer : IRenderer
{
    private CommonCamera? _camera;
    private ReadOnlyBuffer<Triangle>? _geometryBuffer;

    public RayTracingRenderer(GraphicsDevice device)
    {
        Device = device;
    }

    public GraphicsDevice Device { get; }

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

        // Create the rays from the camera
        Device.For(width, height, new CameraCastShader(size, camera, rayBuffer));

        // Find object collision and cache the resulting ray cast 
        Device.For(width, height, new GeometryCollisionShader(_geometryBuffer, rayBuffer, rayCastBuffer));

        // Dump the ray cast's directions to the render buffer (for debugging)
        Device.For(width, height, new RayCastBufferDumpShader(rayCastBuffer, RenderBuffer, (int)RayCastDumpValueType.Object, 1));
    }

    /// <inheritdoc/>
    public void RenderSegment(int2 offset, int2 size)
    {
        Guard.IsNotNull(RenderBuffer);

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
