using System.Numerics;

namespace RenderSharp.Scenes.Materials
{
    public class DiffuseMaterial : MaterialBase
    {
        public DiffuseMaterial(Vector4 albedo, float roughness = 1) :
            this(string.Empty, albedo, roughness)
        {
        }

        public DiffuseMaterial(string name, Vector4 albedo, float roughness = 1) :
            base(name)
        {
            Albedo = albedo;
            Roughness = roughness;
        }

        public Vector4 Albedo { get; }

        public float Roughness { get; }
    }
}
