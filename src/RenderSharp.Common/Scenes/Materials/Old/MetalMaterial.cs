using ComputeSharp;

namespace RenderSharp.Common.Scenes.Materials
{
    public class MetalMaterial : IMaterial
    {
        public MetalMaterial(Float4 albedo, float roughness)
        {
            Albedo = albedo;
            Roughness = roughness;
        }

        public Float4 Albedo { get; }
        public float Roughness { get; }
    }
}
