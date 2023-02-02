﻿// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Scene.BVH;
using RenderSharp.RayTracing.Scene.Rays;
using System.Numerics;

namespace RenderSharp.RayTracing.Scene.Geometry;

/// <summary>
/// A single triangle for geometry collision.
/// </summary>
public struct Triangle
{
    // TODO: Vertex Normals

    public float3 a, b, c;
    public int matId;
    public int objId;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Triangle"/> struct.
    /// </summary>
    public Triangle(float3 a, float3 b, float3 c, int matId, int objId)
    {
        this.a = a;
        this.b = b;
        this.c = c;
        this.matId = matId;
        this.objId = objId;
    }
    
    /// <remarks>
    /// TODO: Replace with constructor
    /// Waiting on https://github.com/Sergio0694/ComputeSharp/issues/481
    /// </remarks>
    public static Triangle Create(float3 a, float3 b, float3 c, int matId, int objId)
    {
        Triangle tri;
        tri.a = a;
        tri.b = b;
        tri.c = c;
        tri.matId = matId;
        tri.objId = objId;
        return tri;
    }

    /// <remarks>
    /// TODO: Convert to an instance method.
    /// Waiting on https://github.com/Sergio0694/ComputeSharp/issues/479
    /// </remarks>
    public static bool IsHit(Triangle tri, Ray ray, out RayCast cast)
        => IsHit(tri, ray, float.MaxValue, out cast);

    /// <remarks>
    /// TODO: Convert to an instance method.
    /// Waiting on https://github.com/Sergio0694/ComputeSharp/issues/479
    /// </remarks>
    public static bool IsHit(Triangle tri, Ray ray, float maxClip, out RayCast cast)
    {
        // Set default cast values
        cast = RayCast.Create(float3.Zero, float3.Zero, 0);

        // Find the triangle's normal direction
        var normal = Hlsl.Cross(tri.b - tri.a, tri.c - tri.a);

        // TODO: Check if squared length can be used instead for greater performance
        // TODO: Replace with back face culling
        if (Hlsl.Length(normal) < 0)
            return false;

        // Find the length required for the ray to collide with the triangle's plane
        // TODO: Handle perpendicular plane (division by zero?)
        float t = (Hlsl.Dot(normal, tri.a) - Hlsl.Dot(normal, ray.origin)) / Hlsl.Dot(normal, ray.direction);

        // Ensure the collision is in the positive direction, and not outside the clipped range
        if (t < 0.0001f || t > maxClip)
            return false;

        // Find the collision point on the plane
        var q = Ray.PointAt(ray, t);

        // Ensure the ray collides with the triangle's plane within the bounds of the triangle's face
        bool inBounds = Hlsl.Dot(Hlsl.Cross(tri.b - tri.a, q - tri.a), normal) >= 0 &&
                       Hlsl.Dot(Hlsl.Cross(tri.c - tri.b, q - tri.b), normal) >= 0 &&
                       Hlsl.Dot(Hlsl.Cross(tri.a - tri.c, q - tri.c), normal) >= 0;

        if (!inBounds)
            return false;


        cast = RayCast.Create(q, normal, t);
        return true;
    }

    /// <remarks>
    /// TODO: Convert to an instance method.
    /// Waiting on https://github.com/Sergio0694/ComputeSharp/issues/479
    /// </remarks>
    public static AABB GetAABB(Triangle tri)
    {
        Vector3 high = Vector3.Zero;
        Vector3 low = Vector3.Zero;

        for (int axis = 0; axis < 3; axis++)
        {
            high[axis] = MathF.Max(MathF.Max(tri.a[axis], tri.b[axis]), tri.c[axis]);
            low[axis] = MathF.Min(MathF.Min(tri.a[axis], tri.b[axis]), tri.c[axis]);
        }

        return AABB.Create(high, low);
    }
}