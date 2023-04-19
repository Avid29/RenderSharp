// Adam Dernis 2023

using System.Numerics;

namespace RenderSharp.RayTracing.Models.Lighting;

/// <summary>
/// A point light.
/// </summary>
public struct Light
{
    /// <summary>
    /// The light location.
    /// </summary>
    public float3 position;

    /// <summary>
    /// The radiance of the light.
    /// </summary>
    public float4 radiance;

    /// <summary>
    /// Initializes a new instance of the <see cref="Light"/> struct.
    /// </summary>
    /// <param name="position">The light's position.</param>
    /// <param name="color">The light's color.</param>
    /// <param name="power">The intensity of the light.</param>
    public Light(float3 position, Vector3 color, float power)
    {
        this.position = position;
        this.radiance = new Vector4(color * power, 1);
    }
}
