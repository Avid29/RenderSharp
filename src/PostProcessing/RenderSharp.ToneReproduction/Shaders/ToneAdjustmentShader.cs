// Adam Dernis 2023

using ComputeSharp;

namespace RenderSharp.ToneReproduction.Shaders;

/// <summary>
/// A shader that applies an adjustment to each pixel's luminosity.
/// </summary>
[AutoConstructor]
[EmbeddedBytecode(DispatchAxis.XY)]
public partial struct ToneAdjustmentShader : IComputeShader
{
    private readonly IReadWriteNormalizedTexture2D<float4> imageBuffer;
    private readonly ReadWriteTexture2D<float> luminosityBuffer;
    private readonly float dLMax;

    /// <inheritdoc/>
    public void Execute()
    {
        var index2D = ThreadIds.XY;
        imageBuffer[index2D] *= luminosityBuffer[index2D] * 5;
    }
}
