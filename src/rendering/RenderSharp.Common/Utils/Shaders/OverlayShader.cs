using ComputeSharp;
using System.Numerics;

namespace RenderSharp.Utils.Shaders
{
    [AutoConstructor]
    [EmbeddedBytecode(DispatchAxis.XY)]
    public partial struct OverlayShader : IComputeShader
    {
        int2 offset;
        ReadWriteTexture2D<Vector4> overlay;
        ReadWriteTexture2D<Vector4> baseTexture;

        private bool IsWithin(int2 pos, int2 offset, int2 bottomRight)
        {
            if (pos.X < offset.X || pos.X > bottomRight.X)
            {
                return false;
            }
            else if (pos.Y < offset.Y || pos.Y > bottomRight.Y)
            {
                return false;
            }
            return true;
        }

        public void Execute()
        {
            int2 size = new int2(overlay.Width, overlay.Height);
            int2 buttomRight = size + offset;

            int2 pos = ThreadIds.XY;
            if (IsWithin(pos, offset, buttomRight))
            {
                baseTexture[pos] = overlay[pos - offset];
            }
        }
    }
}
