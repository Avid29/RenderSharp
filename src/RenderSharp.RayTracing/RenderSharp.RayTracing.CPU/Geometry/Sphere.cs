using RenderSharp.RayTracing.CPU.Materials;
using RenderSharp.RayTracing.CPU.Rays;
using RenderSharp.RayTracing.CPU.Utils;
using System;
using System.Numerics;

namespace RenderSharp.RayTracing.CPU.Geometry
{
    public struct Sphere : IGeometry
    {
        public Sphere(Vector3 center, float radius, IMaterial material)
        {
            Center = center;
            Radius = radius;
            Material = material;
        }

        public Vector3 Center { get; }

        public float Radius { get; }

        public IMaterial Material { get; }

        public bool IsHit(Ray ray, out RayCast cast)
        {
            return IsHit(ray, float.MaxValue, out cast);
        }

        public bool IsHit(Ray ray, float maxClip, out RayCast cast)
        {
            cast = new RayCast();

            Vector3 oc = ray.Origin - Center;
            float a = FloatUtils.LengthSquared(ray.Direction);
            float b = Vector3.Dot(oc, ray.Direction);
            float c = FloatUtils.LengthSquared(oc) - Radius * Radius;

            float disc = b * b - a * c;

            // If disc < 0, there's no hit
            if (disc < 0) return false;

            float sqrtD = MathF.Sqrt(disc);

            // Check first hit distance
            float dist = (-b - sqrtD) / a;
            if (dist < 0.0001f || dist > maxClip)
            {
                // Check second hit distance
                dist = (-b + sqrtD) / a;
                if (dist < 0.0001f || dist > maxClip)
                {
                    // Neither root is in acceptable range
                    return false;
                }
            }

            // Build ray cast
            Vector3 castPoint = ray.PointAt(dist);
            Vector3 castNormal = (castPoint - Center) / Radius;
            cast = new RayCast(castPoint, castNormal, dist);
            return true;
        }
    }
}
