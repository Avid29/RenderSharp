using ComputeSharp;

namespace RenderSharp.Common.Scenes.Skys
{
    public class Sky
    {
        public Sky(Float4 color)
        {
            Color = color;
        }

        public Float4 Color { get; }
    }
}
