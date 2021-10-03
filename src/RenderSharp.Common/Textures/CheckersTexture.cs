using ComputeSharp;

namespace RenderSharp.Common.Textures
{
    public class CheckersTexture : ITexture
    {
        public CheckersTexture(Float4 color1, Float4 color2)
        {
            Color1 = color1;
            Color2 = color2;
        }

        public Float4 Color1 { get; }

        public Float4 Color2 { get; }
    }
}
