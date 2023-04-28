// Adam Dernis 2023

using ComputeSharp;

namespace RenderSharp.ToneReproduction.Shaders;

/// <summary>
/// A shader that applies a Ward Tone Reproduction scale factor.
/// </summary>
[AutoConstructor]
[EmbeddedBytecode(DispatchAxis.XY)]
public readonly partial struct WardToneReproductionShader : IComputeShader
{
    private readonly IReadWriteNormalizedTexture2D<float4> imageBuffer;
    private readonly float scaleFactor;
    private readonly float lMax;

    /// <inheritdoc />
    public void Execute()
    {
        var index2D = ThreadIds.XY;

        imageBuffer[index2D].XYZ *= scaleFactor * lMax;
    }
}
