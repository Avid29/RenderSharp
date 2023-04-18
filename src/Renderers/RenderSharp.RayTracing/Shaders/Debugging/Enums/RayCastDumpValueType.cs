// Adam Dernis 2023

using RenderSharp.RayTracing.RayCasts;

namespace RenderSharp.RayTracing.Shaders.Debugging.Enums;

/// <summary>
/// An enum for which <see cref="GeometryCollision"/> property to dump.
/// </summary>
public enum RayCastDumpValueType : int
{
    Position,
    Normal,
    SmoothNormal,
    Distance,
    Triangle,
    Object,
}
