// Adam Dernis 2023

using ComputeSharp;

namespace RenderSharp.ToneReproduction.Shaders;

/// <summary>
/// A shader that applies a Reinhard Tone Reproduction.
/// </summary>
[AutoConstructor]
[EmbeddedBytecode(DispatchAxis.XY)]
public partial struct ReinhardToneReproductionShader : IComputeShader
{
    private readonly IReadWriteNormalizedTexture2D<float4> imageBuffer;
    private readonly float scaleFactor;
    private readonly float lMax;

    /// <inheritdoc />
    public void Execute()
    {
        var index2D = ThreadIds.XY;
        float4 pixel = imageBuffer[index2D];
        float alpha = pixel.W;
        float3 scaled = pixel.XYZ * scaleFactor * lMax;
        scaled /= 1 + scaled;
        imageBuffer[index2D] = new float4(scaled, alpha);
    }
}
