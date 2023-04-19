// Adam Dernis 2023

using ComputeSharp;

namespace RenderSharp.Renderers.Debug.Shaders;

/// <summary>
/// A test shader.
/// </summary>
[AutoConstructor]
[EmbeddedBytecode(DispatchAxis.XY)]
public readonly partial struct Shader : IComputeShader
{
    private readonly IReadWriteNormalizedTexture2D<float4> _texture;
    
    /// <inheritdoc/>
    public void Execute()
    {
        int2 pos = ThreadIds.XY;
        float2 normPos = ThreadIds.Normalized.XY;

        _texture[pos] = new float4(normPos.XY, 0, 1);
    }
}
