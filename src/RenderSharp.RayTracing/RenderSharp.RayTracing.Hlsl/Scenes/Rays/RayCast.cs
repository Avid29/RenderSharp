using ComputeSharp;

namespace RenderSharp.RayTracing.HLSL.Scenes.Rays
{
    public struct RayCast
    {
        public Float3 origin;
        public Float3 normal;
        public float coefficient;

        public static RayCast Create() => Create(Float3.Zero, Float3.Zero, 0);

        public static RayCast Create(Float3 origin, Float3 normal, float coefficient)
        {
            RayCast ray;
            ray.origin = origin;
            ray.normal = normal;
            ray.coefficient = coefficient;
            return ray;
        }

        public static Float3 HitPoint(RayCast ray)
        {
            return ray.origin + ray.coefficient * ray.normal;
        }

        public static Float3 PointAt(RayCast ray, float c)
        {
            return ray.origin + c * ray.normal;
        }
    }
}
