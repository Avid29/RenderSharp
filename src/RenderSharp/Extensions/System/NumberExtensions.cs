// Adam Dernis 2023

namespace System;

/// <summary>
/// A class containing extensions for primitive number types.
/// </summary>
public static class NumberExtensions
{
    /// <summary>
    /// Converts a floating point value in degrees to radians.
    /// </summary>
    /// <param name="value">A floating point value in degrees.</param>
    /// <returns><paramref name="value"/> in radians.</returns>
    public static float ToRadians(this float value)
        => value * MathF.PI / 180;
}
