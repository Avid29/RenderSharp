// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Scene.Rays;

namespace RenderSharp.RayTracing.Scene.Geometry;

/// <summary>
/// A single triangle for geometry collision.
/// </summary>
public struct Triangle
{
    public float3 a, b, c;
    public int objId;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Triangle"/> struct.
    /// </summary>
    public Triangle(float3 a, float3 b, float3 c, int objId)
    {
        this.a = a;
        this.b = b;
        this.c = c;
        this.objId = objId;
    }
    
    /// <remarks>
    /// TODO: Replace with constructor
    /// Waiting on https://github.com/Sergio0694/ComputeSharp/issues/481
    /// </remarks>
    public static Triangle Create(float3 a, float3 b, float3 c, int objId)
    {
        Triangle tri;
        tri.a = a;
        tri.b = b;
        tri.c = c;
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
            normal *= -1;

        // Find the length required for the ray to collide with the triangle's plane
        // TODO: Handle perpendicular plane (division by zero?)
        float t = (Hlsl.Dot(normal, tri.a) - Hlsl.Dot(normal, ray.origin)) / Hlsl.Dot(normal, ray.direction);

        // Ensure the collision is in the positive direction, and not outside the clipped range
        if (t < 0 || t > maxClip)
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
}
