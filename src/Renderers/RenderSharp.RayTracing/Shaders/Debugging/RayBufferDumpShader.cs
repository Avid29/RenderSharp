// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Models.Rays;

namespace RenderSharp.RayTracing.Shaders.Debugging;

[AutoConstructor]
[EmbeddedBytecode(DispatchAxis.XY)]
public readonly partial struct RayBufferDumpShader : IComputeShader
{
    private readonly ReadWriteBuffer<Ray> rayBuffer;
    private readonly IReadWriteNormalizedTexture2D<float4> renderBuffer;
    private readonly bool dumpDirection;

    public void Execute()
    {
        // Get the index of resources managed by the current thread
        // in both 2D textures and flat buffers
        int2 index2D = ThreadIds.XY;
        int fIndex = (index2D.Y * DispatchSize.X) + index2D.X;

        // If dumpDirection get the ray direction
        // If not dumpDirection get the ray origin
        var ray = rayBuffer[fIndex];
        float3 dumpValue = dumpDirection ? ray.direction : ray.origin;

        // Dump the origin or direction with a full alpha channel
        renderBuffer[index2D] = new float4(dumpValue, 1);
    }
}
