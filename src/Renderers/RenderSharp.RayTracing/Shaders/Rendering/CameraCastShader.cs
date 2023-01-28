// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Scene.Rays;
using RenderSharp.RayTracing.Scene.Camera;

namespace RenderSharp.RayTracing.Shaders.Rendering;

/// <summary>
/// An <see cref="IComputeShader"/> that creates camera rays.
/// </summary>
[AutoConstructor]
[EmbeddedBytecode(DispatchAxis.XY)]
public readonly partial struct CameraCastShader : IComputeShader
{
    private readonly int2 size;
    private readonly Camera camera;
    private readonly ReadWriteBuffer<Ray> rayBuffer;

    public void Execute()
    {
        // Get the index of resources managed by the current thread
        // in both 2D textures and flat buffers
        int2 index2D = ThreadIds.XY;
        int fIndex = (index2D.Y * DispatchSize.X) + index2D.X;

        // Calculate the camera u and v normalized pixel coordinates.
        float u = (index2D.X + 0.5f) / DispatchSize.X;
        float v = 1 - (index2D.Y + 0.5f) / DispatchSize.Y;

        // Create a ray from the camera and store it in the ray buffer.
        var ray = Camera.CreateRay(camera, u, v);
        rayBuffer[fIndex] = ray;
    }
}
