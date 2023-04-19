// Adam Dernis 2023

namespace RenderSharp.RayTracing.Models.Geometry;

/// <summary>
/// A triangle for geometry collision.
/// </summary>
public struct Triangle
{
    /// <summary>
    /// The triangle vertex indices in the vertex buffer.
    /// </summary>
    public int a, b, c;

    /// <summary>
    /// The material id of the triangle's shader.
    /// </summary>
    public int matId;

    /// <summary>
    /// The id of the object the triangle belongs to.
    /// </summary>
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
}
