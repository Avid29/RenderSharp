using ComputeSharp;
using RenderSharp.WinUI.Renderer;

namespace RenderSharp.Common.Devices.Buffers
{
    public class CPUReadWriteImageBuffer : IReadWriteImageBuffer
    {
        // Stored as height,width, because that's it will flipp when copied to the GPU buffer
        private Float4[,] _pixels;

        public CPUReadWriteImageBuffer(int width, int height)
        {
            _pixels = new Float4[height, width];
        }

        public int Width => _pixels.GetLength(1);

        public int Height => _pixels.GetLength(0);

        public Float4 this[int x, int y]
        {
            get => _pixels[y, x];
            set => _pixels[y, x] = value;
        }

        public void CopyToGPU(ReadWriteTexture2D<Float4> output)
        {
            CopyToGPU(output, Int2.Zero, new Int2(output.Width, output.Height));
        }

        public void CopyToGPU(ReadWriteTexture2D<Float4> output, Int2 offset, Int2 size)
        {
            var buffer = Gpu.Default.AllocateReadWriteTexture2D(_pixels);
            Gpu.Default.For(size.X, size.Y, new OverlayShader(offset, size, buffer, output));
        }
    }
}
