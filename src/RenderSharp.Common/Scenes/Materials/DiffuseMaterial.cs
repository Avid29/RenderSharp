using System.Numerics;

namespace RenderSharp.Scenes.Materials
{
    public class DiffuseMaterial : IMaterial
    {
        public DiffuseMaterial(Vector4 albedo, float roughness = 1)
        {
            Albedo = albedo;
            Roughness = roughness;
        }

        public Vector4 Albedo { get; }

        public float Roughness { get; }
    }
}
