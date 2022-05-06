using System.Numerics;

namespace RenderSharp.Scenes.Materials
{
    public class GlossyMaterial : MaterialBase
    {
        public GlossyMaterial(Vector4 albedo, float roughness) :
            this(string.Empty, albedo, roughness)
        {
            Albedo = albedo;
            Roughness = roughness;
        }

        public GlossyMaterial(string name, Vector4 albedo, float roughness) :
            base(name)
        {
            Albedo = albedo;
            Roughness = roughness;
        }

        public Vector4 Albedo { get; }

        public float Roughness { get; }
    }
}
