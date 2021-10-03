using RenderSharp.Common.Textures;

namespace RenderSharp.Common.Materials
{
    public class DiffuseMaterial : IMaterial
    {
        public DiffuseMaterial(ITexture albedo, float roughness = 1)
        {
            Albedo = albedo;
            Roughness = roughness;
        }

        public ITexture Albedo { get; }

        public float Roughness { get; }
    }
}
