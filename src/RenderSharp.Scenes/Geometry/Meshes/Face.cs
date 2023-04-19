// Adam Dernis 2023

namespace RenderSharp.Scenes.Geometry.Meshes;

/// <summary>
/// A face for the common RenderSharp scene.
/// </summary>
public class Face
{
    // TODO: Polygon support

    /// <summary>
    /// Initializes a new instance of the <see cref="Face"/> class.
    /// </summary>
    /// <param name="a">The first vertex.</param>
    /// <param name="b">The second vertex.</param>
    /// <param name="c">The third vertex.</param>
    public Face(Vertex a, Vertex b, Vertex c)
    {
        A = a;
        B = b;
        C = c;
    }

    /// <summary>
    /// Gets or sets the first vertex.
    /// </summary>
    public Vertex A { get; set; }
    
    /// <summary>
    /// Gets or sets the second vertex.
    /// </summary>
    public Vertex B { get; set; }
    
    /// <summary>
    /// Gets or sets the third vertex.
    /// </summary>
    public Vertex C { get; set; }
}
