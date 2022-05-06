using ComputeSharp;
using System.Numerics;

namespace RenderSharp.Utils.Shaders
{
    [AutoConstructor]
    [EmbeddedBytecode(DispatchAxis.XY)]
    public partial struct OverlayShader : IComputeShader
    {
        Int2 offset;
        ReadWriteTexture2D<Vector4> overlay;
        ReadWriteTexture2D<Vector4> baseTexture;

        private bool IsWithin(Int2 pos, Int2 offset, Int2 bottomRight)
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
            Int2 size = new Int2(overlay.Width, overlay.Height);
            Int2 buttomRight = size + offset;

            Int2 pos = ThreadIds.XY;
            if (IsWithin(pos, offset, buttomRight))
            {
                baseTexture[pos] = overlay[pos - offset];
            }
        }
    }
}
