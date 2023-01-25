// Adam Dernis 2023

using ComputeSharp;

namespace RenderSharp.Rendering;

public interface IRenderer
{
    GraphicsDevice Device { get; }

    IReadWriteNormalizedTexture2D<float4>? RenderBuffer { get; }

    void Setup(IReadWriteNormalizedTexture2D<float4> renderBuffer);

    void Render();

    void RenderSegment(int2 offset, int2 size);
}
