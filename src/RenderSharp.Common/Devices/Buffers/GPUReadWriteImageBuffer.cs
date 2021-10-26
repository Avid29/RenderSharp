using ComputeSharp;
using RenderSharp.WinUI.Renderer;

namespace RenderSharp.Common.Devices.Buffers
{
    public class GPUReadWriteImageBuffer : IReadWriteImageBuffer
    {
        private ReadWriteTexture2D<Float4> _buffer;

        public GPUReadWriteImageBuffer(int width, int height) : this(Gpu.Default, width, height)
        { }

        public GPUReadWriteImageBuffer(GraphicsDevice gpu, int width, int height)
        {
            _buffer = gpu.AllocateReadWriteTexture2D<Float4>(width, height);
        }

        public GPUReadWriteImageBuffer(ReadWriteTexture2D<Float4> buffer)
        {
            _buffer = buffer;
        }

        public ReadWriteTexture2D<Float4> Buffer => _buffer;

        public int Width => _buffer.Width;

        public int Height => _buffer.Height;

        public void CopyToGPU(ReadWriteTexture2D<Float4> output)
        {
            CopyToGPU(output, Int2.Zero, new Int2(output.Width, output.Height));
        }

        public void CopyToGPU(ReadWriteTexture2D<Float4> output, Int2 offset, Int2 size)
        {
            Gpu.Default.For(size.X, size.Y, new OverlayShader(offset, size, _buffer, output));
        }
    }
}
