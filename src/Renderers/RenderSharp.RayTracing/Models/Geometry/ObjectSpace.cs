// Adam Dernis 2023

namespace RenderSharp.RayTracing.Models.Geometry;

/// <summary>
/// A struct representing an object space transformation.
/// </summary>
public struct ObjectSpace
{
    /// <summary>
    /// The inverse transformation matrix for the object's space.
    /// </summary>
    public float4x4 inverseTransformation;
}
