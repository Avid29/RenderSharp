using RenderSharp.RayTracing.Scenes.Rays;
using System;
using System.Numerics;

namespace RenderSharp.RayTracing.Scenes.BVH
{
    public struct AABB
    {
        public Vector3 maximum;
        public Vector3 minimum;

        public static AABB Create(Vector3 maximum, Vector3 minimum)
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
                float invD = 1f / ((float3)ray.direction)[axis];
                float t0 = (((float3)box.minimum)[axis] - ((float3)ray.origin)[axis]) * invD;
                float t1 = (((float3)box.maximum)[axis] - ((float3)ray.origin)[axis]) * invD;

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
#if NET5_0_OR_GREATER
            x = MathF.Min(box1.minimum.X, box2.minimum.X);
            y = MathF.Min(box1.minimum.Y, box2.minimum.Y);
            z = MathF.Min(box1.minimum.Z, box2.minimum.Z);
#elif NETSTANDARD2_0_OR_GREATER
            x = Math.Min(box1.minimum.X, box2.minimum.X);
            y = Math.Min(box1.minimum.Y, box2.minimum.Y);
            z = Math.Min(box1.minimum.Z, box2.minimum.Z);
#endif
            Vector3 min = new Vector3(x, y, z);

#if NET5_0_OR_GREATER
            x = MathF.Max(box1.maximum.X, box2.maximum.X);
            y = MathF.Max(box1.maximum.Y, box2.maximum.Y);
            z = MathF.Max(box1.maximum.Z, box2.maximum.Z);
#elif NETSTANDARD2_0_OR_GREATER
            x = Math.Max(box1.maximum.X, box2.maximum.X);
            y = Math.Max(box1.maximum.Y, box2.maximum.Y);
            z = Math.Max(box1.maximum.Z, box2.maximum.Z);
#endif
            Vector3 max = new Vector3(x, y, z);

            return Create(max, min);
        }

        public static bool IsHitOld(AABB box, Ray ray, float maxClip, float minClip)
        {
            for (int axis = 0; axis < 3; axis++)
            {
                float invD = 1f / ((float3)ray.direction)[axis];
                float t0 = (((float3)box.minimum)[axis] - ((float3)ray.origin)[axis]) * invD;
                float t1 = (((float3)box.maximum)[axis] - ((float3)ray.origin)[axis]) * invD;

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
