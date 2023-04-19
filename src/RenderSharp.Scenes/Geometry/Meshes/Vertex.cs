// Adam Dernis 2023

using System.Numerics;

namespace RenderSharp.Scenes.Geometry.Meshes;

/// <summary>
/// A RenderSharp common vertex type.
/// </summary>
public class Vertex
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Vertex"/> class.
    /// </summary>
    /// <param name="position">The vertex position.</param>
    public Vertex(Vector3 position)
    {
        Position = position;
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Vertex"/> class.
    /// </summary>
    /// <param name="position">The vertex position.</param>
    /// <param name="normal">The vertex normal.</param>
    public Vertex(Vector3 position, Vector3 normal) : this(position)
    {
        Normal = normal;
    }

    /// <summary>
    /// Gets or sets the vertex position.
    /// </summary>
    public Vector3 Position { get; set; }

    /// <summary>
    /// Gets or sets the vertex normals.
    /// </summary>
    public Vector3 Normal { get; set; }
}
