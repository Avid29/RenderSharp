// Adam Dernis 2023

using ComputeSharp;

namespace RenderSharp.ToneReproduction.Shaders;

/// <summary>
/// A shader that applies a Tone Reproduction scale factor.
/// </summary>
[AutoConstructor]
[EmbeddedBytecode(DispatchAxis.XY)]
public readonly partial struct ScaleLuminosityShader : IComputeShader
{
    private readonly IReadWriteNormalizedTexture2D<float4> _imageBuffer;
    private readonly float _scaleFactor;

    /// <inheritdoc />
    public void Execute()
    {
        var index2D = ThreadIds.XY;

        _imageBuffer[index2D].XYZ *= _scaleFactor;
    }
}
