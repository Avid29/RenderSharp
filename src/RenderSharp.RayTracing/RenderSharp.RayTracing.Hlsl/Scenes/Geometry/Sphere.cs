using ComputeSharp;
using RenderSharp.RayTracing.HLSL.Scenes.Rays;
using RenderSharp.RayTracing.HLSL.Utils;
using System;
using System.Numerics;

namespace RenderSharp.RayTracing.HLSL.Scenes.Geometry
{
    public struct Sphere
    {
        public Float3 center;
        public float radius;
        public int matId;

        public static bool IsHit(Sphere sphere, RayCast ray, out RayCast cast)
        {
            return IsHit(sphere, float.MaxValue, ray, out cast);
        }

        public static bool IsHit(Sphere sphere, float maxClip, RayCast ray, out RayCast cast)
        {
            // Set cast defaults
            cast.coefficient = 0;
            cast.origin = Float3.Zero;
            cast.normal = Float3.Zero;

            Float3 oc = ray.origin - sphere.center;
            float a = FloatUtils.LengthSquared(ray.normal);
            float b = Vector3.Dot(oc, ray.normal);
            float c = FloatUtils.LengthSquared(oc) - sphere.radius * sphere.radius;

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
            cast.coefficient = dist;
            cast.origin = RayCast.PointAt(ray, dist);
            cast.normal = (cast.origin - sphere.center) / sphere.radius;
            return true;
        }
    }
}
