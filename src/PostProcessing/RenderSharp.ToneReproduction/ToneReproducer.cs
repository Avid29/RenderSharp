// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using ComputeSharp;
using System.Linq;
using RenderSharp.Rendering.Interfaces;
using RenderSharp.ToneReproduction.Shaders;
using System.Net.NetworkInformation;
using RenderSharp.Rendering.Analyzer.Interfaces;

namespace RenderSharp.ToneReproduction;

/// <summary>
/// An <see cref="IPostProcessor"/> implementation that uses tone reproduction to post process an image.
/// </summary>
public class ToneReproducer : IPostProcessor
{
    /// <inheritdoc />
    public GraphicsDevice? Device { get; set; }

    /// <inheritdoc />
    public IRenderAnalyzer? RenderAnalyzer { get; set; }

    /// <inheritdoc />
    public void Process(IReadWriteNormalizedTexture2D<float4> buffer)
    {
        Guard.IsNotNull(Device);

        float lMax = 1000f;
        float lDMax = 500f;
        bool ward = true;

        var height = buffer.Height;
        var width = buffer.Width;
        var luminosityBuffer = Device.AllocateReadWriteTexture2D<float>(width, height);
        Device.For(width, height, new CalculateLuminosityShader(buffer, luminosityBuffer, lMax));

        // TODO: Calculate log of each pixel on GPU
        float[,] pixelLumins = new float[height, width];
        luminosityBuffer.CopyTo(pixelLumins);

        var logAvg = LogAverage(pixelLumins);

        if (ward)
        {
            var scale = WardScaleFactor(lDMax, logAvg);
            Device.For(width, height, new WardToneReproductionShader(buffer, scale, lMax));
        }
        else
        {
            var scale = ReinhardScaleFactor(lDMax, logAvg, 0.18f);
            Device.For(width, height, new ReinhardToneReproductionShader(buffer, scale, lMax));
        }
    }

    private static float WardScaleFactor(float lDMax, float logAvg)
    {
        float numerator = 1.219f + MathF.Pow(lDMax / 2, 0.4f);
        float denominator = 1.219f + MathF.Pow(logAvg, 0.4f);
        return MathF.Pow(numerator / denominator, 2.5f) / lDMax;
    }

    private static float ReinhardScaleFactor(float lDMax, float logAvg, float keyValue)
        => keyValue * lDMax / logAvg;

    private static float LogAverage(float[,] pixels, float delta = 0.0001f)
    {
        float sum = pixels.Cast<float>().Sum(pixel => MathF.Log(delta + pixel));
        return MathF.Exp(sum / pixels.Length);
    }
}
