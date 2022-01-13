using System.Numerics;

namespace RenderSharp.Scenes.Skys
{
    public class Sky
    {
        public Sky(Vector4 color)
        {
            Color = color;
        }

        public Vector4 Color { get; }
    }
}
