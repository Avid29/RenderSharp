// Adam Dernis 2023

using ComputeSharp;

namespace RenderSharp.RayTracing.Shaders.Debugging;

/// <summary>
/// An <see cref="IComputeShader"/> that dumps a property from a <see cref="float4"/> buffer as a color.
/// </summary>
[AutoConstructor]
[EmbeddedBytecode(DispatchAxis.XY)]
public readonly partial struct Float4DumpShader : IComputeShader
{
    private readonly IReadWriteNormalizedTexture2D<float4> attenuationBuffer;
    private readonly IReadWriteNormalizedTexture2D<float4> renderBuffer;

    /// <inheritdoc/>
    public void Execute()
    {
        renderBuffer[ThreadIds.XY] = attenuationBuffer[ThreadIds.XY];
    }
}
