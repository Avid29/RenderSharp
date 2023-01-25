// Adam Dernis 2023

using ComputeSharp;

namespace RenderSharp.Extensions.ComputeSharp.Shaders;

[AutoConstructor]
[EmbeddedBytecode(DispatchAxis.XY)]
public partial struct CopyShader : IComputeShader
{
    private readonly IReadWriteNormalizedTexture2D<Float4> _source;
    private readonly IReadWriteNormalizedTexture2D<Float4> _destination;

    private readonly int2 _sourceOffset;
    private readonly int2 _destinationOffset;

    public void Execute()
    {
        int2 relativePosition = ThreadIds.XY;
        int2 sourcePosition = relativePosition + _sourceOffset;
        int2 destinationPosition = relativePosition + _destinationOffset;

        _destination[destinationPosition] = _source[sourcePosition];
    }
}
