// Adam Dernis 2023

using ComputeSharp;

namespace RenderSharp.ToneReproduction.Shaders;

/// <summary>
/// A shader that calculates the luminosity of pixels in an image.
/// </summary>
[AutoConstructor]
[EmbeddedBytecode(DispatchAxis.XY)]
public partial struct CalculateLuminosityShader : IComputeShader
{
    private readonly IReadWriteNormalizedTexture2D<float4> imageBuffer;
    private readonly ReadWriteTexture2D<float> luminosityBuffer;
    private readonly float lMax;

    /// <inheritdoc/>
    public void Execute()
    {
        var lCo = new float3x1(0.27f, 0.67f, 0.06f);

        var index2D = ThreadIds.XY;
        var pixel4 = imageBuffer[index2D];
        var pixel = new float1x3(pixel4.X, pixel4.Y, pixel4.Z);
        //luminosityBuffer[index2D] = Hlsl.Log(((pixel * lCo).M11 * lMax) + 0.0001f);
        luminosityBuffer[index2D] = (pixel * lMax * lCo).M11;
    }
}
