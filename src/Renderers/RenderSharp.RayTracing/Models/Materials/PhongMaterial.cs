// Adam Dernis 2023

using System.Numerics;

namespace RenderSharp.RayTracing.Models.Materials;

public struct PhongMaterial
{
    public float4 diffuse;
    public float4 specular;
    public float4 ambient;

    public PhongMaterial(Vector4 diffuse, Vector4 specular, Vector4 ambient,
        float cDiffuse = 1, float cSpecular = 1, float cAmbient = 1)
    {
        this.diffuse = diffuse * cDiffuse;
        this.specular = specular * cSpecular;
        this.ambient = ambient * cAmbient;
    }
}
