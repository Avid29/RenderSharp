// Adam Dernis 2023

using RenderSharp.RayTracing.RayCasts;

namespace RenderSharp.RayTracing.Shaders.Debugging.Enums;

/// <summary>
/// An enum for which <see cref="GeometryCollision"/> property to dump.
/// </summary>
public enum GeometryCollisionDumpValueType : int
{
#pragma warning disable CS1591
    Position,
    Normal,
    SmoothNormal,
    Distance,
    Triangle,
    Object,
#pragma warning restore CS1591
}
