// Adam Dernis 2023

using RenderSharp.Scenes.Geometry.Meshes;

namespace RenderSharp.Scenes.Geometry.Interfaces;

/// <summary>
/// An interface for geometry objects in a RenderSharp scene.
/// </summary>
public interface IGeometry
{
    /// <summary>
    /// Converts the <see cref="IGeometry"/> object into a <see cref="Mesh"/>.
    /// </summary>
    /// <returns>The <see cref="IGeometry"/> object as a <see cref="Mesh"/>.</returns>
    Mesh ConvertToMesh();
    
    /// <summary>
    /// Applies a transformation to the <see cref="IGeometry"/>.
    /// </summary>
    /// <param name="transformation">The transformation to apply.</param>
    void ApplyTransformation(Transformation transformation);
}
