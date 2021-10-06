using ComputeSharp;

namespace RenderSharp.Common.Devices.Buffers
{
    public interface IReadWriteImageBuffer
    {
        int Width { get; }

        int Height { get; }

        public void CopyToGPU(ReadWriteTexture2D<Float4> output);
    }
}
