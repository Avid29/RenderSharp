// Adam Dernis 2023

using RenderSharp.Scenes.Geometry.Interfaces;
using System.Collections.Generic;

namespace RenderSharp.Scenes.Geometry.Meshes;

/// <summary>
/// A mesh for the common RenderSharp scene.
/// </summary>
public class Mesh : IGeometry
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Mesh"/> class.
    /// </summary>
    public Mesh()
    {
        Vertices = new List<Vertex>();
        Faces = new List<Face>();
    }

    /// <summary>
    /// Gets or sets the mesh's vertices. 
    /// </summary>
    public List<Vertex> Vertices { get; set; }
    
    /// <summary>
    /// Gets or sets the mesh's faces. 
    /// </summary>
    public List<Face> Faces { get; set; }

    /// <inheritdoc/>
    public Mesh ConvertToMesh() => this;
    
    /// <inheritdoc/>
    public void ApplyTransformation(Transformation transformation)
    {
        throw new System.NotImplementedException();
    }
}
