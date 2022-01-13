namespace RenderSharp.RayTracing.Scenes.Materials
{
    public struct DiffuseMaterial
    {
        public float4 albedo;
        public float roughness;

        public static DiffuseMaterial Create(float4 albedo, float roughness)
        {
            DiffuseMaterial material;
            material.albedo = albedo;
            material.roughness = roughness;
            return material;
        }
    }
}
