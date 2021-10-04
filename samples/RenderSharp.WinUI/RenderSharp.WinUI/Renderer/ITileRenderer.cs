using ComputeSharp;
using CommonScene = RenderSharp.Common.Components.Scene;

namespace RenderSharp.WinUI.Renderer
{
    public interface ITileRenderer
    {
        public void AllocateResources(CommonScene scene);

        public void Render(IReadWriteTexture2D<Float4> texture, Int2 size, Int2 offset);
    }
}
