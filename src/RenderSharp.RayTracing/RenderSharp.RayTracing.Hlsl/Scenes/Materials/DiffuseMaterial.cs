using ComputeSharp;

namespace RenderSharp.RayTracing.HLSL.Scenes.Materials
{
    public struct DiffuseMaterial
    {
        public Float4 albedo;
        public float roughness;

        public static DiffuseMaterial Create(Float4 albedo, float roughness)
        {
            DiffuseMaterial material;
            material.albedo = albedo;
            material.roughness = roughness;
            return material;
        }
    }
}
