// Adam Dernis 2023

using System.Collections.Generic;

namespace RenderSharp.Scenes.Meshes;

public class Mesh
{
    public Mesh()
    {
        Vertices = new List<Vertex>();
    }

    public List<Vertex> Vertices { get; set; }
}
