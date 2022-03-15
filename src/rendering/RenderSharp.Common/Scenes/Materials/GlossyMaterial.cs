using System.Numerics;

namespace RenderSharp.Scenes.Materials
{
    public class GlossyMaterial : IMaterial
    {
        public GlossyMaterial(Vector4 albedo, float roughness)
        {
            Albedo = albedo;
            Roughness = roughness;
        }

        public Vector4 Albedo { get; }

        public float Roughness { get; }
    }
}
