using ComputeSharp;
using RenderSharp.RayTracing.Scenes.BVH;
using RenderSharp.RayTracing.Scenes.Rays;
using RenderSharp.RayTracing.Utils;
using System;
using System.Numerics;

namespace RenderSharp.RayTracing.Scenes.Geometry
{
    public struct Triangle
    {
        public Vector3 a, b, c;
        public int matId;

        public static Triangle Create(Vector3 a, Vector3 b, Vector3 c, int matId)
        {
            Triangle triangle;
            triangle.a = a;
            triangle.b = b;
            triangle.c = c;
            triangle.matId = matId;
            return triangle;
        }

        public static bool IsHit(Triangle tri, Ray ray, out RayCast cast)
        {
            return IsHit(tri, float.MaxValue, ray, out cast);
        }

        public static bool IsHit(Triangle tri, float maxClip, Ray ray, out RayCast cast)
        {
            // Set cast defaults
            cast.coefficient = 0;
            cast.origin = Vector3.Zero;
            cast.normal = Vector3.Zero;

            // Find the direction normal to the triangle
            // (normal)
            Vector3 normal = Vector3.Cross(tri.b - tri.a, tri.c - tri.a);
            if (FloatUtils.LengthSquared(normal) < 0)
            {
                normal *= -1;
            }

            // Find the ray's distance from the triangle's plane
            // (coefficient)
            float d = Vector3.Dot(normal, tri.a);
            float t = (d - Vector3.Dot(normal, ray.origin)) / Vector3.Dot(normal, ray.direction);

            if (t < 0.0001f || t > maxClip) return false;

            // Find the collision point to the plane
            // (origin)
            Vector3 q = Ray.PointAt(ray, t);

            // Ensure the collision point is in the bounds of the face
            if (Vector3.Dot(Vector3.Cross(tri.b - tri.a, q - tri.a), normal) < 0) return false;
            if (Vector3.Dot(Vector3.Cross(tri.c - tri.b, q - tri.b), normal) < 0) return false;
            if (Vector3.Dot(Vector3.Cross(tri.a - tri.c, q - tri.c), normal) < 0) return false;

            // Build ray cast
            cast.origin = q;
            cast.normal = normal;
            cast.coefficient = t;
            return true;
        }

        public static AABB GetBoundingBox(Triangle triangle)
        {
            float x, y, z;
#if NET5_0_OR_GREATER
            x = MathF.Min(triangle.a.X, MathF.Min(triangle.b.X, triangle.c.X));
            y = MathF.Min(triangle.a.Y, MathF.Min(triangle.b.Y, triangle.c.Y));
            z = MathF.Min(triangle.a.Z, MathF.Min(triangle.b.Z, triangle.c.Z));
#elif NETSTANDARD2_0_OR_GREATER
            x = Math.Min(triangle.a.X, Math.Min(triangle.b.X, triangle.c.X));
            y = Math.Min(triangle.a.Y, Math.Min(triangle.b.Y, triangle.c.Y));
            z = Math.Min(triangle.a.Z, Math.Min(triangle.b.Z, triangle.c.Z));
#endif
            Vector3 min = new Vector3(x, y, z);

#if NET5_0_OR_GREATER
            x = MathF.Max(triangle.a.X, MathF.Max(triangle.b.X, triangle.c.X));
            y = MathF.Max(triangle.a.Y, MathF.Max(triangle.b.Y, triangle.c.Y));
            z = MathF.Max(triangle.a.Z, MathF.Max(triangle.b.Z, triangle.c.Z));
#elif NETSTANDARD2_0_OR_GREATER
            x = Math.Max(triangle.a.X, Math.Max(triangle.b.X, triangle.c.X));
            y = Math.Max(triangle.a.Y, Math.Max(triangle.b.Y, triangle.c.Y));
            z = Math.Max(triangle.a.Z, Math.Max(triangle.b.Z, triangle.c.Z));
#endif
            Vector3 max = new Vector3(x, y, z);

            return AABB.Create(max, min);
        }
    }
}
