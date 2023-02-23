
// Adam Dernis 2023

using ComputeSharp;

namespace RenderSharp.RayTracing.Shaders.Rendering;

[AutoConstructor]
[EmbeddedBytecode(DispatchAxis.XY)]
public partial struct SampleCopyShader : IComputeShader
{ 
    private readonly IReadWriteNormalizedTexture2D<float4> colorBuffer;
    private readonly IReadWriteNormalizedTexture2D<float4> colorSumBuffer;

    /// <inheritdoc/>
    public void Execute()
    {
        var index2D = ThreadIds.XY;
        colorSumBuffer[index2D] += colorBuffer[index2D];
    }
}
