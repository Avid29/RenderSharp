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

        public int Width => _buffer.Width;

        public int Height => _buffer.Height;

        public void CopyToGPU(ReadWriteTexture2D<Float4> output)
        {
            Gpu.Default.For(output.Width, output.Height, new OverlayShader(new Int2(0, 0), _buffer, output));
        }
    }
}
