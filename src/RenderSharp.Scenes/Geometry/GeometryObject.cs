// Adam Dernis 2023

using RenderSharp.Scenes.Geometry.Meshes;

namespace RenderSharp.Scenes.Geometry;

/// <summary>
/// A base class for geometry objects for the common RenderSharp scene.
/// </summary>
public abstract class GeometryObject : Object
{
    /// <summary>
    /// Converts the <see cref="GeometryObject"/> object into a <see cref="Mesh"/>.
    /// </summary>
    /// <returns>The <see cref="GeometryObject"/> object as a <see cref="Mesh"/>.</returns>
    public abstract Mesh ConvertToMesh();
}
