using ComputeSharp;

namespace RenderSharp.Common.Objects
{
    public class Sphere : IObject
    {
        public Sphere(Float3 center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        public Float3 Center { get; }

        public float Radius { get; }
    }
}
