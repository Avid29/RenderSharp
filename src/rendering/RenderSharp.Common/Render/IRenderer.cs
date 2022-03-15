using RenderSharp.Buffer;
using RenderSharp.Render.Tiles;
using RenderSharp.Scenes;

namespace RenderSharp.Render
{
    public interface IRenderer
    {
        public void Setup(Scene scene, int imageWidth, int imageHeight);

        public void RenderTile(Tile tile);

        public IReadWriteImageBuffer Buffer { get; }

        public bool IsCPU { get; }
    }
}
