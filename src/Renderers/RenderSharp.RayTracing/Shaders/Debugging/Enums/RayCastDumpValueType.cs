// Adam Dernis 2023

using RenderSharp.RayTracing.Models.Rays;

namespace RenderSharp.RayTracing.Shaders.Debugging.Enums;

/// <summary>
/// An enum for which <see cref="GeometryCollision"/> property to dump.
/// </summary>
public enum RayCastDumpValueType : int
{
    Position,
    Normal,
    Distance,
    Triangle,
    Object,
}
