// Adam Dernis 2023

namespace RenderSharp.RayTracing.Shaders.Pipeline.Collision.Enums;

/// <summary>
/// An enum for collision detection modes.
/// </summary>
public enum CollisionMode
{
    /// <summary>
    /// Find the nearest geometry collision, if any.
    /// </summary>
    Nearest,

    /// <summary>
    /// Find any geometry collision, if any.
    /// </summary>
    Any,
}
