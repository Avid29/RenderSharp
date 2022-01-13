using ComputeSharp;
using RenderSharp.Buffer;
using RenderSharp.RayTracing.GPU.Shaders;
using RenderSharp.Render.Tiles;
using CommonScene = RenderSharp.Scenes.Scene;

namespace RenderSharp.RayTracing.GPU
{
    public class RayTracer
    {
        private GPUReadWriteImageBuffer _buffer;
        private int2 _fullSize;

        public RayTracer(int2 fullsize, GPUReadWriteImageBuffer buffer)
        {
            _fullSize = fullsize;
            _buffer = buffer;
        }

        public void RenderTile(Tile tile)
        {
            GraphicsDevice.Default.For(tile.Width, tile.Height, new TestShader(tile.Offset, _fullSize, _buffer.Buffer));
        }
    }
}
