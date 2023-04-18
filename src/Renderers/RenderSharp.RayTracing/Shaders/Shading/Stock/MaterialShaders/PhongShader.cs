﻿// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Models.Geometry;
using RenderSharp.RayTracing.Models.Lighting;
using RenderSharp.RayTracing.Models.Materials;
using RenderSharp.RayTracing.RayCasts;
using RenderSharp.RayTracing.Shaders.Shading.Interfaces;

namespace RenderSharp.RayTracing.Shaders.Shading.Stock.MaterialShaders;

[EmbeddedBytecode(DispatchAxis.XY)]
public partial struct PhongShader : IMaterialShader
{
    private readonly int matId;
    private readonly PhongMaterial material;
    
#nullable disable
    private ReadOnlyBuffer<ObjectSpace> objectBuffer;
    private ReadOnlyBuffer<Light> lightBuffer;
    private ReadWriteBuffer<Ray> pathRayBuffer;
    private ReadWriteBuffer<GeometryCollision> pathCastBuffer;
    private ReadWriteBuffer<Ray> shadowRayBuffer;
    private ReadWriteBuffer<GeometryCollision> shadowCastBuffer;
    private IReadWriteNormalizedTexture2D<float4> attenuationBuffer;
    private IReadWriteNormalizedTexture2D<float4> colorBuffer;
#nullable restore

    public PhongShader(int matId, PhongMaterial material)
    {
        this.matId = matId;
        this.material = material;
    }

    /// <inheritdoc/>
    public void Execute()
    {
        // Get the index of resources managed by the current thread
        // in both 2D textures and flat buffers
        int2 index2D = ThreadIds.XY;
        int fIndex = (index2D.Y * DispatchSize.X) + index2D.X;
        
        var cast = pathCastBuffer[fIndex];
        var ray = pathRayBuffer[fIndex];

        // If this material was not hit do not execute
        if (cast.matId != matId)
            return;

        // Calculate diffuse and specular intensity
        float4 diffuseIntensity = float4.Zero;
        float4 specularIntensity = float4.Zero;
        for (int i = 0; i < lightBuffer.Length; i++)
        {
            var fShadowIndex = (i * DispatchSize.X * DispatchSize.Y) + (index2D.Y * DispatchSize.X) + index2D.X; 
            
            if (shadowCastBuffer[fShadowIndex].geoId != -1)
                continue;

            var l = shadowRayBuffer[fShadowIndex].direction;
            
            var n = cast.smoothNormal;
            var v = ray.direction;
            var r = Hlsl.Reflect(l, cast.smoothNormal);
            
            diffuseIntensity += lightBuffer[i].color * Hlsl.Max(Hlsl.Dot(n, l), 0f);
            specularIntensity += lightBuffer[i].color * Hlsl.Pow(Hlsl.Max(Hlsl.Dot(r, ray.direction), 0), material.roughness);
        }

        var att = attenuationBuffer[index2D];

        // Sum ambient, diffuse, and specular components
        colorBuffer[index2D] += material.ambient;
        colorBuffer[index2D] += material.diffuse * diffuseIntensity;
        colorBuffer[index2D] += material.specular * specularIntensity;
        attenuationBuffer[index2D] = 0;
    }

    ReadOnlyBuffer<ObjectSpace> IMaterialShader.ObjectBuffer  { set => objectBuffer = value; }

    ReadOnlyBuffer<Light> IMaterialShader.LightBuffer  { set => lightBuffer = value; }

    ReadWriteBuffer<Ray> IMaterialShader.PathRayBuffer { set => pathRayBuffer = value; }

    ReadWriteBuffer<GeometryCollision> IMaterialShader.PathCastBuffer {  set => pathCastBuffer = value; }

    ReadWriteBuffer<Ray> IMaterialShader.ShadowRayBuffer { set => shadowRayBuffer = value; }

    ReadWriteBuffer<GeometryCollision> IMaterialShader.ShadowCastBuffer { set => shadowCastBuffer = value; }

    IReadWriteNormalizedTexture2D<float4> IMaterialShader.AttenuationBuffer { set => attenuationBuffer = value; }

    IReadWriteNormalizedTexture2D<float4> IMaterialShader.ColorBuffer { set => colorBuffer = value; }
}
