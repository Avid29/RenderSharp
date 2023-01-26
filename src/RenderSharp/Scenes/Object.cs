// Adam Dernis 2023

using RenderSharp.Scenes.Meshes;

namespace RenderSharp.Scenes;

public class Object
{
    public Object(Mesh mesh)
    {
        Mesh = mesh;
    }

    public Mesh Mesh { get; set; }

    public Transformation Transformation { get; set; }
}
