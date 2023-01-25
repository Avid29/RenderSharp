using CommunityToolkit.Diagnostics;
using ComputeSharp;
using RenderSharp.Renderers.Debug.Shaders;
using RenderSharp.Rendering;

namespace RenderSharp.Renderers.Debug;

public class SimpleRenderer : IRenderer
{
    private IReadWriteNormalizedTexture2D<float4>? _buffer;

    public SimpleRenderer(GraphicsDevice device)
    {
        Device = device;
    }

    public GraphicsDevice Device { get; set; }

    public IReadWriteNormalizedTexture2D<float4>? RenderBuffer => _buffer;

    public void Setup(IReadWriteNormalizedTexture2D<float4> renderBuffer)
    {
        _buffer = renderBuffer;
    }

    public void Render()
    {
        Guard.IsNotNull(RenderBuffer);

        Device.For(RenderBuffer.Width, RenderBuffer.Height, new Shader(RenderBuffer));
    }

    public void RenderSegment(int2 offset, int2 size)
    {
        Guard.IsNotNull(RenderBuffer);
    }
}
