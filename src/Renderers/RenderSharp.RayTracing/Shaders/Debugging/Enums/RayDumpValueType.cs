// Adam Dernis 2023

using RenderSharp.RayTracing.RayCasts;

namespace RenderSharp.RayTracing.Shaders.Debugging.Enums;

/// <summary>
/// An enum for which <see cref="Ray"/> property to dump.
/// </summary>
public enum RayDumpValueType : int
{
#pragma warning disable CS1591
    Origin,
    Direction,
#pragma warning restore CS1591
}
