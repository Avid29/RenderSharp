// Adam Dernis 2023

using ComputeSharp;

namespace RenderSharp.Extensions.ComputeSharp.Shaders;

/// <summary>
/// A shader that copies a section of data from one <see cref="IReadWriteNormalizedTexture2D{TPixel}"/> to another.
/// </summary>
[AutoConstructor]
[EmbeddedBytecode(DispatchAxis.XY)]
public readonly partial struct CopyShader : IComputeShader
{
    private readonly IReadWriteNormalizedTexture2D<float4> _source;
    private readonly IReadWriteNormalizedTexture2D<float4> _destination;

    private readonly int2 _sourceOffset;
    private readonly int2 _destinationOffset;

    /// <inheritdoc/>
    public void Execute()
    {
        int2 relativePosition = ThreadIds.XY;
        int2 sourcePosition = relativePosition + _sourceOffset;
        int2 destinationPosition = relativePosition + _destinationOffset;

        _destination[destinationPosition] = _source[sourcePosition];
    }
}
