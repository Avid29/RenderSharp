// Adam Dernis 2023

using System.Numerics;

namespace RenderSharp.RayTracing.Scene.Materials;
public struct PhongMaterial
{
    public float3 diffuse;
    public float3 specular;
    public float3 ambient;

    public PhongMaterial(Vector3 diffuse, Vector3 specular, Vector3 ambient,
        float cDiffuse = 1, float cSpecular = 1, float cAmbient = 1)
    {
        this.diffuse = diffuse * cDiffuse;
        this.specular = specular * cSpecular;
        this.ambient = ambient * cAmbient;
    }
}
