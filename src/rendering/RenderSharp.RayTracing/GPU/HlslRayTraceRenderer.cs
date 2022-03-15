using RenderSharp.Buffer;
using RenderSharp.Render;
using RenderSharp.Render.Tiles;
using RenderSharp.Scenes;

namespace RenderSharp.RayTracing.GPU
{
    public class HlslRayTraceRenderer : IRenderer
    {
        private GPUReadWriteImageBuffer _buffer;
        private RayTracer _rayTracer;

        public IReadWriteImageBuffer Buffer => _buffer;

        public bool IsCPU => false;

        public void RenderTile(Tile tile)
        {
            _rayTracer.RenderTile(tile);
        }

        public void Setup(Scene scene, int imageWidth, int imageHeight)
        {
            _buffer = new GPUReadWriteImageBuffer(imageWidth, imageHeight);

            _rayTracer = new RayTracer(scene, new int2(imageWidth, imageHeight), _buffer);
        }
    }
}
