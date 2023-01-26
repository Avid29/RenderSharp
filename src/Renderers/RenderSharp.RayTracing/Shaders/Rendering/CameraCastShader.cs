// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Models.Rays;
using RenderSharp.RayTracing.Models.Camera;

namespace RenderSharp.RayTracing.Shaders.Rendering;

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

        var uvPos = ThreadIds.Normalized.XY;
        var ray = Camera.CreateRay(camera, uvPos.X, 1 - uvPos.Y);
        rayBuffer[fIndex] = ray;
    }
}
