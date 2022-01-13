using ComputeSharp;

namespace RenderSharp.Buffer
{
    public interface IReadWriteImageBuffer
    {
        int Width { get; }

        int Height { get; }

        public void CopyToGPU(ReadWriteTexture2D<Float4> output);
    }
}
