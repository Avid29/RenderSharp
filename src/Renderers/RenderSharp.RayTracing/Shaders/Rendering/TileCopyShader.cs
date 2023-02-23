// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.Utilities.Tiles;

namespace RenderSharp.RayTracing.Shaders.Rendering;

[AutoConstructor]
[EmbeddedBytecode(DispatchAxis.XY)]
public partial struct TileCopyShader : IComputeShader
{
    private readonly Tile _tile;
    private readonly IReadWriteNormalizedTexture2D<float4> colorSumBuffer;
    private readonly IReadWriteNormalizedTexture2D<float4> renderBuffer;
    private int samples;

    /// <inheritdoc/>
    public void Execute()
    {
        var sourceIndex = ThreadIds.XY;
        var destIndex = sourceIndex + _tile.offset;

        renderBuffer[destIndex] = colorSumBuffer[sourceIndex] / samples;
    }
}
