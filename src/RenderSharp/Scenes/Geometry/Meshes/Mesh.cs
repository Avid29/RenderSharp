// Adam Dernis 2023

using RenderSharp.Scenes.Geometry.Interfaces;
using System.Collections.Generic;

namespace RenderSharp.Scenes.Geometry.Meshes;

public class Mesh : IGeometry
{
    public Mesh()
    {
        Vertices = new List<Vertex>();
        Faces = new List<Face>();
    }

    public List<Vertex> Vertices { get; set; }

    public List<Face> Faces { get; set; }

    public Mesh ConvertToMesh() => this;

    public void ApplyTransformation(Transformation transformation)
    {
        throw new System.NotImplementedException();
    }
}
