namespace RenderSharp.RayTracing.Scenes.Rays
{
    public struct Ray
    {
        public float3 origin;
        public float3 direction;

        public static Ray Create() => Create(float3.Zero, float3.Zero);

        public static Ray Create(float3 origin, float3 direction)
        {
            Ray ray;
            ray.origin = origin;
            ray.direction = direction;
            return ray;
        }

        public static float3 PointAt(Ray ray, float c)
        {
            return ray.origin + c * ray.direction;
        }
    }
}
