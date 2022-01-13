namespace RenderSharp.RayTracing.Scenes.Materials
{
    public struct GlossyMaterial
    {
        public float4 albedo;
        public float roughness;

        public static GlossyMaterial Create(float4 albedo, float roughness)
        {
            GlossyMaterial material;
            material.albedo = albedo;
            material.roughness = roughness;
            return material;
        }
    }
}
