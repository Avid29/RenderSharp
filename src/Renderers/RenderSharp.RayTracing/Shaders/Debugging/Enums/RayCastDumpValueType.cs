// Adam Dernis 2023

using RenderSharp.RayTracing.Scene.Rays;

namespace RenderSharp.RayTracing.Shaders.Debugging.Enums;

/// <summary>
/// An enum for which <see cref="RayCast"/> property to dump.
/// </summary>
public enum RayCastDumpValueType : int
{
    Position,
    Normal,
    Distance,
    Triangle,
    Object,
}
