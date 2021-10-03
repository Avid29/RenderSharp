using ComputeSharp;
using RenderSharp.Common.Textures;

namespace RenderSharp.Common.Materials
{
    public class MetalMaterial : IMaterial
    {
        public MetalMaterial(ITexture albedo, float roughness)
        {
            Albedo = albedo;
            Roughness = roughness;
        }

        public ITexture Albedo { get; }
        public float Roughness { get; }
    }
}
