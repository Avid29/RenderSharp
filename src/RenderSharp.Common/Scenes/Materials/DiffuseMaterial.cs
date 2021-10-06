using ComputeSharp;

namespace RenderSharp.Common.Scenes.Materials
{
    public class DiffuseMaterial : IMaterial
    {
        public DiffuseMaterial(Float4 albedo, float roughness = 1)
        {
            Albedo = albedo;
            Roughness = roughness;
        }

        public Float4 Albedo { get; }

        public float Roughness { get; }
    }
}
