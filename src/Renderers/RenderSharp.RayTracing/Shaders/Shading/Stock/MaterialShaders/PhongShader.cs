// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Models.Lighting;
using RenderSharp.RayTracing.Models.Materials;
using RenderSharp.RayTracing.Models.Rays;
using RenderSharp.Utilities.Tiles;
using System;

namespace RenderSharp.RayTracing.Shaders.Shading.Stock.MaterialShaders;

[AutoConstructor]
[EmbeddedBytecode(DispatchAxis.XY)]
public partial struct PhongShader : IComputeShader
{
    private readonly int matId;
    private readonly PhongMaterial material;

    private readonly ReadOnlyBuffer<Light> lightsBuffer;
    private readonly ReadWriteBuffer<Ray> rayBuffer;
    private readonly ReadWriteBuffer<Ray> shadowCastBuffer;
    private readonly ReadWriteBuffer<GeometryCollision> rayCastBuffer;
    private readonly IReadWriteNormalizedTexture2D<float4> colorBuffer;
    
    /// <inheritdoc/>
    public void Execute()
    {
        // Get the index of resources managed by the current thread
        // in both 2D textures and flat buffers
        int2 index2D = ThreadIds.XY;
        int fIndex = (index2D.Y * DispatchSize.X) + index2D.X;
        
        var cast = rayCastBuffer[fIndex];
        var ray = rayBuffer[fIndex];

        // If this material was not hit do not execute
        if (cast.matId != matId)
            return;

        // Calculate diffuse and specular intensity
        float4 diffuseIntensity = float4.Zero;
        float4 specularIntensity = float4.Zero;
        for (int i = 0; i < lightsBuffer.Length; i++)
        {
            var fShadowIndex = (i * DispatchSize.X * DispatchSize.Y) + (index2D.Y * DispatchSize.X) + index2D.X; 
            var dir = shadowCastBuffer[fShadowIndex].direction;
            if (Hlsl.Length(dir) == 0)
                break;

            diffuseIntensity += lightsBuffer[i].color * Hlsl.Dot(cast.normal, shadowCastBuffer[fShadowIndex].direction);
            specularIntensity += lightsBuffer[i].color * Hlsl.Pow(Hlsl.Dot(cast.normal, Hlsl.Normalize(dir - ray.direction)), material.roughness);
        }

        // Sum ambient, diffuse, and specular components
        colorBuffer[index2D] += material.ambient;
        colorBuffer[index2D] += material.diffuse * diffuseIntensity;
        colorBuffer[index2D] += material.specular * specularIntensity;
    }
}
