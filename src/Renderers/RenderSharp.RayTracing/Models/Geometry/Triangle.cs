// Adam Dernis 2023

using ComputeSharp;

namespace RenderSharp.RayTracing.Models.Geometry;

/// <summary>
/// A single triangle for geometry collision.
/// </summary>
public struct Triangle
{
    public int a, b, c;
    public int matId;
    public int objId;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Triangle"/> struct.
    /// </summary>
    public Triangle(int a, int b, int c, int matId, int objId)
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
    public static Triangle Create(int a, int b, int c, int matId, int objId)
    {
        Triangle tri;
        tri.a = a;
        tri.b = b;
        tri.c = c;
        tri.matId = matId;
        tri.objId = objId;
        return tri;
    }

    public static VertexTriangle LoadVertexTriangle(Triangle triangle, List<Vertex> vertexBuffer)
    {
        VertexTriangle vTri;
        vTri.triangle = triangle;
        vTri.a = vertexBuffer[triangle.a];
        vTri.b = vertexBuffer[triangle.b];
        vTri.c = vertexBuffer[triangle.c];
        return vTri;
    }

    public static VertexTriangle LoadVertexTriangle(Triangle triangle, ReadOnlyBuffer<Vertex> vertexBuffer)
    {
        VertexTriangle vTri;
        vTri.triangle = triangle;
        vTri.a = vertexBuffer[triangle.a];
        vTri.b = vertexBuffer[triangle.b];
        vTri.c = vertexBuffer[triangle.c];
        return vTri;
    }
}
