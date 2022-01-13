using System.Numerics;

namespace RenderSharp.Scenes.Skys
{
    public class Sky
    {
        public Sky(Vector4 albedo)
        {
            Albedo = albedo;
        }

        public Vector4 Albedo { get; }
    }
}
