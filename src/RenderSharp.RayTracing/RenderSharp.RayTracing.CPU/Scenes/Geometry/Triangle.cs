using RenderSharp.RayTracing.CPU.Scenes.BVH;
using RenderSharp.RayTracing.CPU.Scenes.Materials;
using RenderSharp.RayTracing.CPU.Scenes.Rays;
using System;
using System.Numerics;

namespace RenderSharp.RayTracing.CPU.Scenes.Geometry
{
    public struct Triangle : IGeometry
    {
        public Triangle(Vector3 a, Vector3 b, Vector3 c, IMaterial material)
        {
            A = a;
            B = b;
            C = c;
            Material = material;
        }

        public Vector3 A { get; }

        public Vector3 B { get; }
        
        public Vector3 C { get; }

        public IMaterial Material { get; }

        public bool IsHit(Ray ray, out RayCast cast)
        {
            return IsHit(ray, float.MaxValue, out cast);
        }

        public bool IsHit(Ray ray, float maxClip, out RayCast cast)
        {
            cast = new RayCast();

            Vector3 normal = Vector3.Cross(B - A, C - A);
            if (normal.LengthSquared() < 0) normal *= -1;

            // Find the ray's distance from the triangle's plane
            float d = Vector3.Dot(normal, A);
            float t = (d - Vector3.Dot(normal, ray.Origin)) / Vector3.Dot(normal, ray.Direction);

            if (t < 0.0001f || t > maxClip) return false;

            // Find the collision point to the plane
            Vector3 q = ray.PointAt(t);

            // Ensure the collision point is in the bounds of the face
            if (Vector3.Dot(Vector3.Cross(B - A, q - A), normal) < 0) return false;
            if (Vector3.Dot(Vector3.Cross(C - B, q - B), normal) < 0) return false;
            if (Vector3.Dot(Vector3.Cross(A - C, q - C), normal) < 0) return false;

            cast = new RayCast(q, normal, t);
            return true;
        }

        public AABB GetBoundingBox()
        {
            float x, y, z;
            x = MathF.Min(A.X, MathF.Min(B.X, C.X));
            y = MathF.Min(A.Y, MathF.Min(B.Y, C.Y));
            z = MathF.Min(A.Z, MathF.Min(B.Z, C.Z));
            Vector3 min = new Vector3(x, y, z);

            x = MathF.Max(A.X, MathF.Max(B.X, C.X));
            y = MathF.Max(A.Y, MathF.Max(B.Y, C.Y));
            z = MathF.Max(A.Z, MathF.Max(B.Z, C.Z));
            Vector3 max = new Vector3(x, y, z);

            return new AABB(max, min);
        }
    }
}
