// Adam Dernis 2023

using RenderSharp.Scenes.Geometry.Interfaces;
using RenderSharp.Scenes.Geometry.Meshes;

namespace RenderSharp.Scenes.Geometry;

/// <summary>
/// A generic geometry object type for the common RenderSharp scene.
/// </summary>
/// <typeparam name="T">The type of the geometry object.</typeparam>
public class GeometryObject<T> : GeometryObject
    where T : IGeometry
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GeometryObject{T}"/> class.
    /// </summary>
    /// <param name="geometry">The object geometry.</param>
    public GeometryObject(T geometry)
    {
        Geometry = geometry;
    }

    /// <summary>
    /// Gets or sets the object geometry.
    /// </summary>
    public T Geometry { get; set; }

    // TODO: Apply transformations!
    /// <inheritdoc/>
    public override Mesh ConvertToMesh()
        => Geometry.ConvertToMesh();
}
