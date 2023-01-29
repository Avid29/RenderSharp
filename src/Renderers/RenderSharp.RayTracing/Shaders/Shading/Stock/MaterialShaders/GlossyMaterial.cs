// Adam Dernis 2023

namespace RenderSharp.RayTracing.Shaders.Shading.Stock.MaterialShaders;

/// <summary>
/// Parameters for a <see cref="GlossyShader"/>.
/// </summary>
public struct GlossyMaterial
{
    public float4 albedo;
    public float roughness;
}
