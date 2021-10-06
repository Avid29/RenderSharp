using System.Numerics;

namespace RenderSharp.Common.Scenes.Materials
{
    public class SuperMaterial : IMaterial
    {
        public string Name { get; set; }

        public Vector4 Albedo { get; set; }
    }
}
