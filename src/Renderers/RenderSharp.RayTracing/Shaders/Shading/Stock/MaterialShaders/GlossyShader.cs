﻿// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Models.Geometry;
using RenderSharp.RayTracing.Models.Lighting;
using RenderSharp.RayTracing.RayCasts;
using RenderSharp.RayTracing.Shaders.Shading.Interfaces;

namespace RenderSharp.RayTracing.Shaders.Shading.Stock.MaterialShaders;

[EmbeddedBytecode(DispatchAxis.XY)]
public partial struct GlossyShader : IMaterialShader
{
    private readonly int matId;
    
#nullable disable
    private ReadOnlyBuffer<ObjectSpace> objectBuffer;
    private ReadOnlyBuffer<Light> lightBuffer;
    private ReadWriteBuffer<Ray> pathRayBuffer;
    private ReadWriteBuffer<GeometryCollision> pathCastBuffer;
    private ReadWriteBuffer<Ray> shadowRayBuffer;
    private ReadWriteBuffer<GeometryCollision> shadowCastBuffer;
    private IReadWriteNormalizedTexture2D<float4> colorBuffer;
#nullable restore

    public GlossyShader(int matId)
    {
        this.matId = matId;
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

        var r = Hlsl.Reflect(ray.direction, cast.smoothNormal);
        pathRayBuffer[fIndex] = Ray.Create(cast.position, r);
    }
    

    ReadOnlyBuffer<ObjectSpace> IMaterialShader.ObjectBuffer  { set => objectBuffer = value; }

    ReadOnlyBuffer<Light> IMaterialShader.LightBuffer  { set => lightBuffer = value; }

    ReadWriteBuffer<Ray> IMaterialShader.PathRayBuffer { set => pathRayBuffer = value; }

    ReadWriteBuffer<GeometryCollision> IMaterialShader.PathCastBuffer {  set => pathCastBuffer = value; }

    ReadWriteBuffer<Ray> IMaterialShader.ShadowRayBuffer { set => shadowRayBuffer = value; }

    ReadWriteBuffer<GeometryCollision> IMaterialShader.ShadowCastBuffer { set => shadowCastBuffer = value; }

    IReadWriteNormalizedTexture2D<float4> IMaterialShader.AttenuationBuffer { set => _ = value; }

    IReadWriteNormalizedTexture2D<float4> IMaterialShader.ColorBuffer { set => colorBuffer = value; }
}
