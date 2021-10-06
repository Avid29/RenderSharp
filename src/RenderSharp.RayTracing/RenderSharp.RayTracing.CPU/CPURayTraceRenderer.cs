using RenderSharp.Common.Devices.Buffers;
using RenderSharp.Common.Render;
using RenderSharp.Common.Render.Tiles;
using RenderSharp.RayTracing.CPU.Conversion;
using RenderSharp.RayTracing.CPU.Scenes;
using CommonScene = RenderSharp.Common.Scenes.Scene;

namespace RenderSharp.RayTracing.CPU
{
    public class CPURayTraceRenderer : IRenderer
    {
        private CPUReadWriteImageBuffer _buffer;
        private RayTracer _rayTracer;

        public IReadWriteImageBuffer Buffer => _buffer;

        public void Setup(CommonScene scene, int imageWidth, int imageHeight)
        {
            SceneConverter converter = new SceneConverter();
            Scene _scene = converter.ConvertScene(scene);

            _buffer = new CPUReadWriteImageBuffer(imageWidth, imageHeight);
            _rayTracer = new RayTracer(_scene, _buffer);
        }

        public void RenderTile(Tile tile)
        {
            _rayTracer.RenderTile(tile);
        }
    }
}
