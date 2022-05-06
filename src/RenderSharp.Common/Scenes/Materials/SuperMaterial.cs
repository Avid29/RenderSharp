using System.Numerics;

namespace RenderSharp.Scenes.Materials
{
    public class SuperMaterial : MaterialBase
    {
        public SuperMaterial() :
            base(string.Empty)
        {
        }

        public SuperMaterial(string name) :
            base(name)
        {
        }

        public Vector4 Albedo { get; set; }

        public float Specular { get; set; }

        public Vector4 Emission { get; set; }

        public float Fresnel { get; set; }
    }
}
