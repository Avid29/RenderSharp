using ComputeSharp;

namespace RenderSharp.RayTracing.HLSL.Scenes.Rays
{
    public struct Ray
    {
        public Float3 origin;
        public Float3 direction;

        public static Ray Create() => Create(Float3.Zero, Float3.Zero);

        public static Ray Create(Float3 origin, Float3 direction)
        {
            Ray ray;
            ray.origin = origin;
            ray.direction = direction;
            return ray;
        }

        public static Float3 PointAt(Ray ray, float c)
        {
            return ray.origin + c * ray.direction;
        }

        public static Float2x3 AsMatrix(Ray ray)
        {
            return new Float2x3(ray.origin, ray.direction);
        }
    }
}
