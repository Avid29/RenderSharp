﻿// Adam Dernis 2023

using System.Numerics;

namespace RenderSharp.RayTracing.Models.Materials;

public struct CheckeredPhongMaterial
{
    public float4 diffuse0;
    public float4 diffuse1;
    public float4 specular;
    public float cAmbient;
    public float roughness;
    public float scale;

    public CheckeredPhongMaterial(Vector3 diffuse0, Vector3 diffuse1, Vector3 specular, float roughness, float scale,
        float cDiffuse0 = 1, float cDiffuse1 = 1, float cSpecular = 1, float cAmbient = 1)
    {
        this.diffuse0 = new float4(diffuse0 * cDiffuse0, 1);
        this.diffuse1 = new float4(diffuse1 * cDiffuse1, 1);
        this.specular = new float4(specular * cSpecular, 1);
        this.cAmbient = cAmbient;
        this.scale = scale;
        this.roughness = roughness;
    }
}