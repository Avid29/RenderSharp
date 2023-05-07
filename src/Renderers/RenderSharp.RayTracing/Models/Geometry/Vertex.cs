// Adam Dernis 2023

using System.Numerics;

namespace RenderSharp.RayTracing.Models.Geometry;

/// <summary>
/// A vertex stored on the GPU for the RayTracing renderer.
/// </summary>
public struct Vertex
{
    /// <summary>
    /// The vertex position.
    /// </summary>
    public Vector3 position;

    /// <summary>
    /// The vertex normal.
    /// </summary>
    public Vector3 normal;
    
    ///// <summary>
    ///// The vertex texture coordinates
    ///// </summary>
    //public float2 textureCoords;

    /// <summary>
    /// Initializes a new instance of the <see cref="Vertex"/> struct.
    /// </summary>
    /// <param name="position">The vertex position.</param>
    /// <param name="normal">The vertex normal.</param>
    public Vertex(Vector3 position, Vector3 normal)
    {
        this.position = position;
        this.normal = normal;
        //textureCoords = 0;
    }
}
