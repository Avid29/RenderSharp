// Adam Dernis 2023

using RenderSharp.RayTracing.Models.BVH;
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
