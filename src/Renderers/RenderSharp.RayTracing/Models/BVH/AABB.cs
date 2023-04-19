﻿// Adam Dernis 2023

using RenderSharp.RayTracing.RayCasts;
using System.Numerics;

namespace RenderSharp.RayTracing.Models.BVH;

/// <summary>
/// An axis-aligned bounding box.
/// </summary>
public struct AABB
{
    /// <summary>
    /// The top right front corner.
    /// </summary>
    public Vector3 highCorner;

    /// <summary>
    /// The bottom left back corner.
    /// </summary>
    public Vector3 lowCorner;

    /// <summary>
    /// Checks if a <see cref="Ray"/> intersects an <see cref="AABB"/> within the clipping range.
    /// </summary>
    /// <param name="box">The <see cref="AABB"/>.</param>
    /// <param name="ray">The <see cref="Ray"/>.</param>
    /// <param name="maxClip">The maximum intersection distance.</param>
    /// <param name="minClip">The minimum intersection distance.</param>
    /// <returns>True if the ray intersections in the intersection range.</returns>
    public static bool IsHit(AABB box, Ray ray, float maxClip, float minClip)
    {
        for (int axis = 0; axis < 3; axis++)
        {
            float invD = 1f / ray.direction[axis];
            float t0 = (box.lowCorner[axis] - ray.origin[axis]) * invD;
            float t1 = (box.highCorner[axis] - ray.origin[axis]) * invD;

            if (invD < 0f)
            {
                float swap = t0;
                t0 = t1;
                t1 = swap;
            }

            minClip = MathF.Max(t0, minClip);
            maxClip = MathF.Min(t1, maxClip);

            if (maxClip < minClip)
                return false;
        }

        return true;
    }

    /// <remarks>
    /// TODO: Replace with constructor
    /// Waiting on https://github.com/Sergio0694/ComputeSharp/issues/481
    /// </remarks>
    public static AABB Create(float3 high, float3 low)
    {
        AABB box;
        box.highCorner = high;
        box.lowCorner = low;
        return box;
    }

    /// <summary>
    /// Gets the AABB surrounding two AABBs.
    /// </summary>
    /// <param name="box1">The first AABB.</param>
    /// <param name="box2">The second AABB.</param>
    /// <returns>The surrounding AABB.</returns>
    public static AABB GetSurroundingBox(AABB box1, AABB box2)
    {
        Vector3 high = Vector3.Zero;
        Vector3 low = Vector3.Zero;

        for (int axis = 0; axis < 3; axis++)
        {
            high[axis] = MathF.Max(box1.highCorner[axis], box2.highCorner[axis]);
            low[axis] = MathF.Min(box1.lowCorner[axis], box2.lowCorner[axis]);
        }

        return Create(high, low);
    }
}
