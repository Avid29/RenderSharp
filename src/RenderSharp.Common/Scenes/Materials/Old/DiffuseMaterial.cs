using System.Numerics;

namespace RenderSharp.Common.Scenes.Materials
{
    public class DiffuseMaterial : IMaterial
    {
        public DiffuseMaterial()
        {
            Albedo = new Vector4(0.5f, 0.5f, 0.5f, 1f);
            Roughness = 0.5f;
        }

        public DiffuseMaterial(Vector4 albedo, float roughness = 1)
        {
            Albedo = albedo;
            Roughness = roughness;
        }

        public Vector4 Albedo { get; }

        public float Roughness { get; }
    }
}
