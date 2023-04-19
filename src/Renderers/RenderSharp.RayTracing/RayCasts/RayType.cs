// Adam Dernis 2023

namespace RenderSharp.RayTracing.RayCasts;

/// <summary>
/// A struct containing ray type data
/// </summary>
public struct RayType
{
    /// <summary>
    /// The ray's primary type.
    /// </summary>
    /// <remarks>
    /// 0: Path
    /// 1: Shadow
    ///
    /// TODO: Replace with enum
    /// Waiting on https://github.com/Sergio0694/ComputeSharp/issues/248
    /// </remarks>
    public int rootType;

    /// <summary>
    /// The ray's secondary type.
    /// </summary>
    /// <remarks>
    /// 0: Camera
    /// 1: Diffuse
    /// 2: Reflection
    /// 3: Transmission
    ///
    /// TODO: Replace with enum
    /// Waiting on https://github.com/Sergio0694/ComputeSharp/issues/248
    /// </remarks>
    public int secondaryType;
}
