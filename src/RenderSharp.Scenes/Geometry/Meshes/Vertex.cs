// Adam Dernis 2023

using System.Numerics;

namespace RenderSharp.Scenes.Geometry.Meshes;

public class Vertex
{
    public Vertex(Vector3 position)
    {
        Position = position;
    }

    public Vector3 Position { get; set; }

    public Vector3 Normal { get; set; }
}
