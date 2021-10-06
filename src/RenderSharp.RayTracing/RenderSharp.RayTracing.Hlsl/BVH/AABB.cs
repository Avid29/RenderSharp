using ComputeSharp;
using RenderSharp.RayTracing.HLSL.Rays;
using System;

namespace RenderSharp.RayTracing.HLSL.BVH
{
    public struct AABB
    {
        public Float3 maximum;
        public Float3 minimum;

        public static AABB Create(Float3 maximum, Float3 minimum)
        {
            AABB box;
            box.maximum = maximum;
            box.minimum = minimum;
            return box;
        }

        public static bool IsHit(AABB box, Ray ray, float maxClip, float minClip)
        {
            for (int axis = 0; axis < 3; axis++)
            {
                float invD = 1f / ray.direction[axis];
                float t0 = (box.minimum[axis] - ray.origin[axis]) * invD;
                float t1 = (box.maximum[axis] - ray.origin[axis]) * invD;

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

        public static AABB GetSurroundingBox(AABB box1, AABB box2)
        {
            float x, y, z;
            x = MathF.Min(box1.minimum.X, box2.minimum.X);
            y = MathF.Min(box1.minimum.Y, box2.minimum.Y);
            z = MathF.Min(box1.minimum.Z, box2.minimum.Z);
            Float3 min = new Float3(x, y, z);

            x = MathF.Max(box1.maximum.X, box2.maximum.X);
            y = MathF.Max(box1.maximum.Y, box2.maximum.Y);
            z = MathF.Max(box1.maximum.Z, box2.maximum.Z);
            Float3 max = new Float3(x, y, z);

            return Create(max, min);
        }

        public static bool IsHitOld(AABB box, Ray ray, float maxClip, float minClip)
        {
            for (int axis = 0; axis < 3; axis++)
            {
                float invD = 1f / ray.direction[axis];
                float t0 = (box.minimum[axis] - ray.origin[axis]) * invD;
                float t1 = (box.maximum[axis] - ray.origin[axis]) * invD;

                if (t1 < t0)
                {
                    float swap = t0;
                    t0 = t1;
                    t1 = swap;
                }

                minClip = t0 > minClip ? t0 : minClip;
                maxClip = t1 < maxClip ? t1 : maxClip;

                if (maxClip <= minClip) return false;
            }

            return true;
        }
    }
}
