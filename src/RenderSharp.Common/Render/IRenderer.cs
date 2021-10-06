using RenderSharp.Common.Devices.Buffers;
using RenderSharp.Common.Render.Tiles;
using CommonScene = RenderSharp.Common.Scenes.Scene;

namespace RenderSharp.Common.Render
{
    public interface IRenderer
    {
        public void Setup(CommonScene scene, int imageWidth, int imageHeight);

        public void RenderTile(Tile tile);

        public IReadWriteImageBuffer Buffer { get; }

        public bool IsCPU { get; }
    }
}
