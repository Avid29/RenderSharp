// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Models.BVH;
using RenderSharp.RayTracing.Models.Rays;
using System.Numerics;

namespace RenderSharp.RayTracing.Models.Geometry;

public struct VertexTriangle
{
    public Triangle triangle;
    public Vertex a, b, c;

    /// <remarks>
    /// TODO: Convert to an instance method.
    /// Waiting on https://github.com/Sergio0694/ComputeSharp/issues/479
    /// </remarks>
    public static bool IsHit(VertexTriangle tri, Ray ray, out GeometryCollision cast)
        => IsHit(tri, ray, float.MaxValue, out cast);

    /// <remarks>
    /// TODO: Convert to an instance method.
    /// Waiting on https://github.com/Sergio0694/ComputeSharp/issues/479
    /// </remarks>
    public static bool IsHit(VertexTriangle tri, Ray ray, float maxClip, out GeometryCollision cast)
    {
        // Set default cast values
        cast = GeometryCollision.Create();

        // Find the triangle's normal direction
        var normal = Hlsl.Cross(tri.b.position - tri.a.position, tri.c.position - tri.a.position);

        // TODO: Check if squared length can be used instead for greater performance
        // TODO: Replace with back face culling
        //if (Hlsl.Length(normal) < 0)
        //    return false;

        // Find the length required for the ray to collide with the triangle's plane
        var dn = Hlsl.Dot(normal, ray.direction);
        float t = (Hlsl.Dot(normal, tri.a.position) - Hlsl.Dot(normal, ray.origin)) / dn;
        bool isBackFace = dn > 0;   

        // Ensure the collision is in the positive direction, and not outside the clipped range
        if (t < 0.0001f || t > maxClip)
            return false;

        // Find the collision point on the plane
        var q = Ray.PointAt(ray, t);

        // Calculate barycentric coordinates
        float u = Hlsl.Dot(Hlsl.Cross(tri.b.position - tri.a.position, q - tri.a.position), normal);
        float v = Hlsl.Dot(Hlsl.Cross(tri.c.position - tri.b.position, q - tri.b.position), normal);
        float w = Hlsl.Dot(Hlsl.Cross(tri.a.position - tri.c.position, q - tri.c.position), normal);

        // Ensure the ray collides with the triangle's plane within the bounds of the triangle's face
        if (u < 0 || v < 0 || w < 0)
            return false;

        // Assert u + v + w is approx 1
        //if (1 - (u + v + w) < 0.001)
        //    return false;

        // TODO: Calculate smooth normal
        var smoothNormal = tri.c.normal * u + tri.a.normal * v + tri.b.normal * w;

        if (Hlsl.Length(smoothNormal) == 0)
            smoothNormal = normal;

        cast = GeometryCollision.Create(q, Hlsl.Normalize(normal), Hlsl.Normalize(smoothNormal), new float2(u, v), t, isBackFace);
        return true;
    }

    /// <remarks>
    /// TODO: Convert to an instance method.
    /// Waiting on https://github.com/Sergio0694/ComputeSharp/issues/479
    /// </remarks>
    public static AABB GetAABB(VertexTriangle tri)
    {
        Vector3 high = Vector3.Zero;
        Vector3 low = Vector3.Zero;

        for (int axis = 0; axis < 3; axis++)
        {
            high[axis] = MathF.Max(MathF.Max(((Vector3)tri.a.position)[axis], ((Vector3)tri.b.position)[axis]), ((Vector3)tri.c.position)[axis]);
            low[axis] = MathF.Min(MathF.Min(((Vector3)tri.a.position)[axis], ((Vector3)tri.b.position)[axis]), ((Vector3)tri.c.position)[axis]);
        }

        return AABB.Create(high, low);
    }
}
