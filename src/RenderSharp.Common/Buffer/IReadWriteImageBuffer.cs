using ComputeSharp;
using System.Numerics;

namespace RenderSharp.Buffer
{
    public interface IReadWriteImageBuffer
    {
        int Width { get; }

        int Height { get; }

        public void CopyToGPU(ReadWriteTexture2D<Vector4> output);
    }
}
