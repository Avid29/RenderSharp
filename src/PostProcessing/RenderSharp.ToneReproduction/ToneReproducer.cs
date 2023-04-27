// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using ComputeSharp;
using RenderSharp.Rendering.Interfaces;
using RenderSharp.ToneReproduction.Shaders;

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

        var height = buffer.Height;
        var width = buffer.Width;
        var luminosityBuffer = Device.AllocateReadWriteTexture2D<float>(width, height);

        using var context = Device.CreateComputeContext();
        context.For(width, height, new CalculateLuminosityShader(buffer, luminosityBuffer));
        context.For(width, height, new ToneAdjustmentShader(buffer, luminosityBuffer, 500f));
    }
}
