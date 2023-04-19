// Adam Dernis 2023

using RenderSharp.Scenes.Geometry;

namespace RenderSharp.Scenes;

/// <summary>
/// A base class for objects for the common RenderSharp scene.
/// </summary>
public class Object
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Object"/>.
    /// </summary>
    public Object()
    {
        Transformation = new Transformation();
    }

    /// <summary>
    /// Gets or sets the name of the object.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the object transformation.
    /// </summary>
    public Transformation Transformation { get; set; }
}
