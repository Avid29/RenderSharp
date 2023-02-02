// Adam Dernis 2023

namespace RenderSharp.Scenes.Geometry.Meshes;

public class Face
{
    // TODO: Polygon support

    public Face(Vertex a, Vertex b, Vertex c)
    {
        A = a;
        B = b;
        C = c;
    }

    public Vertex A { get; set; }

    public Vertex B { get; set; }

    public Vertex C { get; set; }
}
