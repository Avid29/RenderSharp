using ComputeSharp;
using RenderSharp.Common.Scenes.Materials;

namespace RenderSharp.Common.Scenes.Objects
{
    public class Sphere : IObject
    {
        public Sphere(Float3 center, float radius, IMaterial material)
        {
            Center = center;
            Radius = radius;
            Material = material;
        }

        public Float3 Center { get; }

        public float Radius { get; }

        public IMaterial Material { get; set; }
    }
}
