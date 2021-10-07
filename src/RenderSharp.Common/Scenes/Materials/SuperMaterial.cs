using System.Numerics;

namespace RenderSharp.Common.Scenes.Materials
{
    public class SuperMaterial : IMaterial
    {
        public string Name { get; set; }

        public Vector4 Albedo { get; set; }

        public Vector4 Emission { get; set; }

        public float Metallic { get; set; }

        public float Specular { get; set; }

        public float Roughness { get; set; }

        public float Fresnel { get; set; }
    }
}
