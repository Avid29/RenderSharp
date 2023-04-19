// Adam Dernis 2023

namespace RenderSharp.RayTracing.Utils;

/// <summary>
/// A class containing float utility methods.
/// </summary>
public class FloatUtils
{
    /// <summary>
    /// Converts from degrees to radians.
    /// </summary>
    /// <param name="degrees">An angle in degrees.</param>
    /// <returns>The angle in radians.</returns>
    public static float DegreesToRadians(float degrees)
        => MathF.PI / 180f * degrees;
}
