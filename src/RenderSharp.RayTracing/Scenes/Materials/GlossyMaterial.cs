using System.Numerics;

namespace RenderSharp.RayTracing.Scenes.Materials
{
    public struct GlossyMaterial
    {
        public Vector4 albedo;
        public float roughness;

        public static GlossyMaterial Create(Vector4 albedo, float roughness)
        {
            GlossyMaterial material;
            material.albedo = albedo;
            material.roughness = roughness;
            return material;
        }
    }
}
