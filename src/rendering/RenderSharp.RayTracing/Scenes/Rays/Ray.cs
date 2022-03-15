using System.Numerics;

namespace RenderSharp.RayTracing.Scenes.Rays
{
    public struct Ray
    {
        public Vector3 origin;
        public Vector3 direction;

        public static Ray Create() => Create(Vector3.Zero, Vector3.Zero);

        public static Ray Create(Vector3 origin, Vector3 direction)
        {
            Ray ray;
            ray.origin = origin;
            ray.direction = direction;
            return ray;
        }

        public static Vector3 PointAt(Ray ray, float c)
        {
            return ray.origin + c * ray.direction;
        }
    }
}
