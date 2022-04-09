using System.Numerics;

namespace RenderSharp.RayTracing.Scenes.Materials
{
    public struct DiffuseMaterial
    {
        public Vector4 albedo;
        public float roughness;

        public static DiffuseMaterial Create(Vector4 albedo, float roughness)
        {
            DiffuseMaterial material;
            material.albedo = albedo;
            material.roughness = roughness;
            return material;
        }
    }
}
