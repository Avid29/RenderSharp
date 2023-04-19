// Adam Dernis 2023

namespace System.Numerics;

/// <summary>
/// A class containing extension methods for <see cref="Matrix4x4"/>.
/// </summary>
public static class Matrix4x4Extensions
{
    /// <summary>
    /// Convert to a <see cref="float4x4"/>.
    /// </summary>
    /// <param name="m">The <see cref="Matrix4x4"/>.</param>
    /// <returns>An equivilent <see cref="float4x4"/>.</returns>
    public static float4x4 ToFloat4x4(this Matrix4x4 m)
    {
        return new float4x4(
            m.M11, m.M12, m.M13, m.M14,
            m.M21, m.M22, m.M23, m.M24,
            m.M31, m.M32, m.M33, m.M34,
            m.M41, m.M42, m.M43, m.M44);
    }
}
