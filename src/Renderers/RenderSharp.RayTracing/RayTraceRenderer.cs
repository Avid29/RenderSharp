// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using ComputeSharp;
using RenderSharp.RayTracing.Models.Rays;
using RenderSharp.Rendering;
using RenderSharp.RayTracing.Shaders;
using RenderSharp.RayTracing.Shaders.Debugging;
using RenderSharp.RayTracing.Models.Camera;
using System.Numerics;

namespace RenderSharp.RayTracing;

public class RayTracingRenderer : IRenderer
{
    public RayTracingRenderer(GraphicsDevice device)
    {
        Device = device;
    }

    public GraphicsDevice Device { get; }

    public IReadWriteNormalizedTexture2D<float4>? RenderBuffer { get; set; }

    /// <inheritdoc/>
    public void SetupScene()
    {
        // TODO: Load 3D scene
    }

    /// <inheritdoc/>
    public void Render()
    {
        Guard.IsNotNull(RenderBuffer);

        int width = RenderBuffer.Width;
        int height = RenderBuffer.Height;
        float ratio = (float)width / height;
        int2 size = new(width, height);
        int pixelCount = width * height;

        // (Debug) Create camera
        var camera = new Camera(Vector3.Zero, -Vector3.UnitZ, 90, 1, ratio);

        // Allocate buffers
        ReadWriteBuffer<Ray> rayBuffer = Device.AllocateReadWriteBuffer<Ray>(pixelCount);

        // Create the rays from the camera
        Device.For(width, height, new CameraCastShader(size, camera, rayBuffer));

        // Dump the ray's directions to the render buffer (for debugging)
        Device.For(width, height, new RayBufferDumpShader(rayBuffer, RenderBuffer, 1));
    }

    /// <inheritdoc/>
    public void RenderSegment(int2 offset, int2 size)
    {
        Guard.IsNotNull(RenderBuffer);

        throw new NotImplementedException();
    }
}
