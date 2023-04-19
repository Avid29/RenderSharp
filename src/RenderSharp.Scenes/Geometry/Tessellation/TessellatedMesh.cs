// Adam Dernis 2023

using RenderSharp.Scenes.Geometry.Interfaces;
using RenderSharp.Scenes.Geometry.Meshes;

namespace RenderSharp.Scenes.Geometry.Tessellation;

/// <summary>
/// A base class for objects to be tessellated for rendering.
/// </summary>
public abstract class TessellatedShape : IGeometry
{
    /// <inheritdoc/>
    public abstract void ApplyTransformation(Transformation transformation);

    /// <summary>
    /// Tessellates the object into a <see cref="Mesh"/>.
    /// </summary>
    /// <returns>The <see cref="TessellatedShape"/> as a <see cref="Mesh"/>.</returns>
    public abstract Mesh ConvertToMesh();
}
