using ComputeSharp;
using RenderSharp.Common.Materials;

namespace RenderSharp.Common.Objects
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
