// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using ComputeSharp;
using RenderSharp.RayTracing.Models.Camera;
using RenderSharp.RayTracing.Models.Geometry;
using RenderSharp.RayTracing.Models.Rays;
using RenderSharp.RayTracing.Shaders.Debugging;
using RenderSharp.RayTracing.Shaders.Debugging.Enums;
using RenderSharp.RayTracing.Shaders.Rendering;
using RenderSharp.Rendering;
using RenderSharp.Scenes;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

using CommonCamera = RenderSharp.Scenes.Cameras.Camera;

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
    public void SetupScene(Scene scene)
    {
        _camera = scene.ActiveCamera;

        // Tessellate Cube
        var v0 = Vector3.Zero;
        var v1 = Vector3.UnitX;
        var v2 = Vector3.UnitY;
        var v3 = Vector3.UnitZ;
        var v4 = v1 + v2;
        var v5 = v2 + v3;
        var v6 = v1 + v3;
        var v7 = Vector3.One;

        var triangles = new Triangle[]
        {
            // Bottom
            new(v0, v1, v2),
            new(v4, v1, v2),
            
            // Face 1
            new(v0, v1, v3),
            new(v6, v1, v3),
            
            // Face 2
            new(v0, v2, v5),
            new(v0, v3, v5),
            
            // Face 3
            new(v2, v4, v7),
            new(v2, v5, v7),
            
            // Face 4
            new(v4, v1, v6),
            new(v4, v7, v6),
            
            // Face 5
            new(v3, v5, v6),
            new(v7, v5, v6),
        };

        _geometryBuffer = Device.AllocateReadOnlyBuffer(triangles);
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
