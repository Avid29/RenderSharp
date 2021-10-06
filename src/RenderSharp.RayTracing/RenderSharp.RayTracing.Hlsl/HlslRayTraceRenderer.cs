using ComputeSharp;
using RenderSharp.Common.Devices.Buffers;
using RenderSharp.Common.Render;
using RenderSharp.Common.Render.Tiles;
using CommonScene = RenderSharp.Common.Scenes.Scene;

namespace RenderSharp.RayTracing.HLSL
{
    public class HlslRayTraceRenderer : IRenderer
    {
        private GPUReadWriteImageBuffer _buffer;
        RayTracer _rayTracer;

        public IReadWriteImageBuffer Buffer => _buffer;

        public bool IsCPU => false;

        public void Setup(CommonScene scene, int imageWidth, int imageHeight)
        {
            _buffer = new GPUReadWriteImageBuffer(imageWidth, imageHeight);

            _rayTracer = new RayTracer(scene, new Int2(imageWidth, imageHeight), _buffer);
        }

        public void RenderTile(Tile tile)
        {
            _rayTracer.RenderTile(tile);
        }
    }
}
