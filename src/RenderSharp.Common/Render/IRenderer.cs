using ComputeSharp;

namespace RenderSharp.Render
{
    public interface IRenderer
    {
        void Render(IReadWriteTexture2D<Float4> buffer);
    }
}
