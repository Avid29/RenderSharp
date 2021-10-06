using RenderSharp.RayTracing.CPU.Scenes.Rays;
using System;
using System.Numerics;

namespace RenderSharp.RayTracing.CPU.Scenes.BVH
{
    public struct AABB
    {
        public AABB(Vector3 maximum, Vector3 minimum)
        {
            Maximum = maximum;
            Minimum = minimum;
        }

        public bool IsHit(Ray ray, float maxClip, float minClip)
        {
            for (int axis = 0; axis < 3; axis++)
            {
                float invD = 1f / ray.Direction.At(axis);
                float t0 = (Minimum.At(axis) - ray.Origin.At(axis)) * invD;
                float t1 = (Maximum.At(axis) - ray.Origin.At(axis)) * invD;

                if (invD < 0.0f)
                {
                    float swap = t0;
                    t0 = t1;
                    t1 = swap;
                }

                minClip = t0 > minClip ? t0 : minClip;
                maxClip = t1 < maxClip ? t1 : maxClip;

                if (maxClip < minClip) return false;
            }

            return true;
        }

        public Vector3 Maximum { get; }

        public Vector3 Minimum { get; }

        public static AABB GetSurroundingBox(AABB box1, AABB box2)
        {
            float x, y, z;
            x = MathF.Min(box1.Minimum.X, box2.Minimum.X);
            y = MathF.Min(box1.Minimum.Y, box2.Minimum.Y);
            z = MathF.Min(box1.Minimum.Z, box2.Minimum.Z);
            Vector3 min = new Vector3(x, y, z);

            x = MathF.Max(box1.Maximum.X, box2.Maximum.X);
            y = MathF.Max(box1.Maximum.Y, box2.Maximum.Y);
            z = MathF.Max(box1.Maximum.Z, box2.Maximum.Z);
            Vector3 max = new Vector3(x, y, z);

            return new AABB(max, min);
        }
    }
}
