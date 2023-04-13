// Adam Dernis 2023

using System.Numerics;

namespace RenderSharp.RayTracing.Models.Materials;

public struct PrincipledMaterial
{
    public float4 diffuse;
    public float4 specular;
    public float4 ambient;
    public float roughness;
    public float metallic;
    public float transmission;
    public float ior;

    public PrincipledMaterial(Vector3 diffuse, Vector3 specular, Vector3 ambient, float roughness, float metallic, float transmission, float ior)
    {
        this.diffuse = new float4(diffuse, 1);
        this.specular = new float4(specular, 1);
        this.ambient = new float4(ambient, 1);
        this.roughness = roughness;
        this.metallic = metallic;
        this.transmission = transmission;
        this.ior = ior;
    }
}
