using ComputeSharp;
using System;

namespace RenderSharp.RayTracing.HLSL.Textures
{
    public struct Texture
    {
        public Float4 color1;
        public Float4 color2;

        public static Texture Create() => Create(Float4.One, Float4.UnitW);

        public static Texture Create(Float4 color1, Float4 color2)
        {
            Texture texture;
            texture.color1 = color1;
            texture.color2 = color2;
            return texture;
        }

        public static Float4 Value(Texture texture, Float2 uv)
        {
            float xScale = 5;
            float yScale = 30;

            float sines = MathF.Sin(xScale * uv.X) * MathF.Sin(yScale * uv.Y);
            if (sines < 0) return texture.color1;
            else return texture.color2;
        }
    }
}
