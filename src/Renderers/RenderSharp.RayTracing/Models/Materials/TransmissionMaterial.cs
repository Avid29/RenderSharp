// Adam Dernis 2023

namespace RenderSharp.RayTracing.Models.Materials;

/// <summary>
/// A struct containing the properties for transmissive material.
/// </summary>
public struct TransmissionMaterial
{
    /// <summary>
    /// The index of refraction.
    /// </summary>
    public float ior;

    /// <summary>
    /// Initializes a new instance of the <see cref="TransmissionMaterial"/> struct.
    /// </summary>
    /// <param name="ior">The index of refraction.</param>
    public TransmissionMaterial(float ior)
    {
        this.ior = ior;
    }
}
