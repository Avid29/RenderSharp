using ComputeSharp;

namespace RenderSharp.Common.Textures
{
    public class SolidTexture : ITexture
    {
        public SolidTexture(Float4 color)
        {
            Color = color;
        }

        public Float4 Color { get; }

        public static implicit operator SolidTexture(Float4 color) => new SolidTexture(color);
    }
}
