using ComputeSharp;
using RenderSharp.Utils.Shaders;

namespace RenderSharp.Buffer
{
    public class GPUReadWriteImageBuffer : IReadWriteImageBuffer
    {
        private ReadWriteTexture2D<Float4> _buffer;

        public GPUReadWriteImageBuffer(int width, int height) : this(GraphicsDevice.Default, width, height)
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
            GraphicsDevice.Default.For(output.Width, output.Height, new OverlayShader(int2.Zero, _buffer, output));
        }
    }
}
