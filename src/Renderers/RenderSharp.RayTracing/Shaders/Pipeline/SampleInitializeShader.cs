// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Models;

namespace RenderSharp.RayTracing.Shaders.Pipeline;

/// <summary>
/// A shader that initializes a buffers for a sample
/// </summary>
[AutoConstructor]
[EmbeddedBytecode(DispatchAxis.XY)]
public readonly partial struct SampleInitializeShader : IComputeShader
{
    private readonly IReadWriteNormalizedTexture2D<float4> attenuationBuffer;
    private readonly IReadWriteNormalizedTexture2D<float4> luminanceBuffer;
    private readonly ReadWriteBuffer<Rand> randBuffer;
    private readonly int sample;
    
    /// <inheritdoc/>
    public void Execute()
    {
        // Get the index of resources managed by the current thread
        // in both 2D textures and flat buffers
        int2 index2D = ThreadIds.XY;
        int fIndex = (index2D.Y * DispatchSize.X) + index2D.X;

        attenuationBuffer[index2D] = float4.One;
        luminanceBuffer[index2D] = float4.Zero;
        randBuffer[fIndex] = Rand.Create(index2D.X, index2D.Y, sample);
    }
}
