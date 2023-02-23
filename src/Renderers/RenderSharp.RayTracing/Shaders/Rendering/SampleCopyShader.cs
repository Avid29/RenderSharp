
// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.Utilities.Tiles;

namespace RenderSharp.RayTracing.Shaders.Rendering;

[AutoConstructor]
[EmbeddedBytecode(DispatchAxis.XY)]
public partial struct SampleCopyShader : IComputeShader
{ 
    private readonly Tile tile;
    private readonly IReadWriteNormalizedTexture2D<float4> colorBuffer;
    private readonly IReadWriteNormalizedTexture2D<float4> RenderBuffer;
    private readonly int samples;

    /// <inheritdoc/>
    public void Execute()
    {
        var sourceIndex = ThreadIds.XY;
        var destinationIndex = sourceIndex + tile.offset;

        RenderBuffer[destinationIndex] += colorBuffer[sourceIndex] / samples;
    }
}
