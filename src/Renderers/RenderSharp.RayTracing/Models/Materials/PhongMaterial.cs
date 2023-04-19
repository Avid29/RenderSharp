// Adam Dernis 2023

using System.Numerics;

namespace RenderSharp.RayTracing.Models.Materials;

/// <summary>
/// A struct containing the properties for principled material.
/// </summary>
public struct PhongMaterial
{
#pragma warning disable CS1591
    public float4 diffuse;
    public float4 specular;
    public float4 ambient;
    public float roughness;
#pragma warning restore CS1591

    /// <summary>
    /// Initializes a new instance of the <see cref="PhongMaterial"/> struct.
    /// </summary>
    public PhongMaterial(Vector3 diffuse, Vector3 specular, Vector3 ambient, float roughness,
        float cDiffuse = 1, float cSpecular = 1, float cAmbient = 1)
    {
        this.diffuse = new float4(diffuse * cDiffuse, 1);
        this.specular = new float4(specular * cSpecular, 1);
        this.ambient = new float4(ambient * cAmbient, 1);
        this.roughness = roughness;
    }
}
