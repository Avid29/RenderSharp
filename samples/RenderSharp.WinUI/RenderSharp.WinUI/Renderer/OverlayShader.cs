using ComputeSharp;

namespace RenderSharp.WinUI.Renderer
{
    [AutoConstructor]
    public partial struct OverlayShader : IPixelShader<Float4>
    {
        Int2 offset;
        ReadOnlyTexture2D<Float4> overlay;
        IReadWriteTexture2D<Float4> fallback;

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

        public Float4 Execute()
        {
            Int2 size = new Int2(overlay.Width, overlay.Width);
            Int2 buttomRight = size + offset;

            Int2 pos = ThreadIds.XY;
            if (IsWithin(pos, offset, buttomRight))
            {
                return overlay[pos + offset];
            } else
            {
                return fallback[pos];
            }
        }
    }
}
