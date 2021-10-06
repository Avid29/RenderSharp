using ComputeSharp;

namespace RenderSharp.RayTracing.HLSL.Scenes.Rays
{
    public struct Ray
    {
        public Float3 origin;
        public Float3 direction;

        public static Ray Create() => Create(Float3.Zero, Float3.Zero);

        public static Ray Create(Float4 origin, Float4 direction) =>
            Create(new Float3(origin.X, origin.Y, origin.Z), new Float3(direction.X, direction.Y, direction.Z));

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

        public static Float2x4 AsMatrix4(Ray ray)
        {
            return new Float2x4(new Float4(ray.origin, 0), new Float4(ray.direction, 0));
        }
    }
}
