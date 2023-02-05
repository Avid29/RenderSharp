// Adam Dernis 2023

using RenderSharp.Scenes.Geometry;

namespace RenderSharp.Scenes;

public class Object
{
    public Object()
    {
        Transformation = new Transformation();
    }

    public string? Name { get; set; }

    public Transformation Transformation { get; set; }
}
