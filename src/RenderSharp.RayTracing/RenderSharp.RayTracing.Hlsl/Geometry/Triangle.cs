using ComputeSharp;
using RenderSharp.RayTracing.HLSL.Rays;

namespace RenderSharp.RayTracing.HLSL.Geometry
{
    public struct Triangle
    {
        public Float3 a, b, c;

        public static bool IsHit(Triangle tri, Ray ray, out RayCast cast)
        {
            return IsHit(tri, float.MaxValue, ray, out cast);
        }

        public static bool IsHit(Triangle tri, float maxClip, Ray ray, out RayCast cast)
        {
            // Set cast defaults
            cast.coefficient = 0;
            cast.origin = Float3.Zero;
            cast.normal = Float3.Zero;

            Float3 normal = Hlsl.Cross(tri.b - tri.a, tri.c - tri.a);
            if (normal.LengthSquared() < 0)
            {
                normal *= -1;
            }

            // Find the ray's distance from the triangle's plane
            float d = Hlsl.Dot(normal, tri.a);
            float t = (d - Hlsl.Dot(normal, ray.origin)) / Hlsl.Dot(normal, ray.direction);

            if (t < 0.0001f || t > maxClip) return false;

            // Find the collision point to the plane
            Float3 q = Ray.PointAt(ray, t);

            // Ensure the collision point is in the bounds of the face
            if (Hlsl.Dot(Hlsl.Cross(tri.b - tri.a, q - tri.a), normal) < 0) return false;
            if (Hlsl.Dot(Hlsl.Cross(tri.c - tri.b, q - tri.b), normal) < 0) return false;
            if (Hlsl.Dot(Hlsl.Cross(tri.a - tri.c, q - tri.c), normal) < 0) return false;

            // Build ray cast
            cast.origin = q;
            cast.normal = normal;
            cast.coefficient = t;
            return true;
        }
    }
}
