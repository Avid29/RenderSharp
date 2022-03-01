using ComputeSharp;
using Microsoft.Toolkit.HighPerformance;
using RenderSharp.Utils.Shaders;

namespace RenderSharp.Buffer
{
    public class CPUReadWriteImageBuffer : IReadWriteImageBuffer
    {
        // Stored as height,width, because that's it will flipp when copied to the GPU buffer
        private float4[,] _pixels;

        public CPUReadWriteImageBuffer(int width, int height)
        {
            _pixels = new float4[height, width];
        }

        public int Width => _pixels.GetLength(1);

        public int Height => _pixels.GetLength(0);

        public float4 this[int x, int y]
        {
            get => _pixels[y, x];
            set => _pixels[y, x] = value;
        }

        public Span2D<float4> AsSpan()
        {
            return new Span2D<float4>(_pixels);
        }

        public void CopyToGPU(ReadWriteTexture2D<Float4> output)
        {
            var buffer = GraphicsDevice.Default.AllocateReadWriteTexture2D(_pixels);
            GraphicsDevice.Default.For(output.Width, output.Height, new OverlayShader(int2.Zero, buffer, output));
        }
    }
}
