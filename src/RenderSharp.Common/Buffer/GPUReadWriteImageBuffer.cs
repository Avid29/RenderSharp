using ComputeSharp;
using RenderSharp.Utils.Shaders;
using System.Numerics;

namespace RenderSharp.Buffer
{
    public class GPUReadWriteImageBuffer : IReadWriteImageBuffer
    {
        private ReadWriteTexture2D<Vector4> _buffer;

        public GPUReadWriteImageBuffer(int width, int height) : this(GraphicsDevice.Default, width, height)
        { }

        public GPUReadWriteImageBuffer(GraphicsDevice gpu, int width, int height)
        {
            _buffer = gpu.AllocateReadWriteTexture2D<Vector4>(width, height);
        }

        public GPUReadWriteImageBuffer(ReadWriteTexture2D<Vector4> buffer)
        {
            _buffer = buffer;
        }

        public ReadWriteTexture2D<Vector4> Buffer => _buffer;

        public int Width => _buffer.Width;

        public int Height => _buffer.Height;

        public void CopyToGPU(ReadWriteTexture2D<Vector4> output)
        {
            GraphicsDevice.Default.For(output.Width, output.Height, new OverlayShader(Int2.Zero, _buffer, output));
        }
    }
}
