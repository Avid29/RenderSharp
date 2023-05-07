// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Models.Geometry;
using RenderSharp.RayTracing.Models.Lighting;
using RenderSharp.RayTracing.Models.Materials;
using RenderSharp.RayTracing.RayCasts;
using RenderSharp.RayTracing.Shaders.Shading.Interfaces;

namespace RenderSharp.RayTracing.Shaders.Shading.Stock.MaterialShaders;

/// <summary>
/// A shader for a glossy material.
/// </summary>
[EmbeddedBytecode(DispatchAxis.XY)]
public partial struct GlossyShader : IMaterialShader<GlossyMaterial>
{
#nullable disable
    private int matId;
    private GlossyMaterial material;
    private ReadOnlyBuffer<ObjectSpace> objectBuffer;
    private ReadOnlyBuffer<Light> lightBuffer;
    private ReadWriteBuffer<Ray> pathRayBuffer;
    private ReadWriteBuffer<GeometryCollision> pathCastBuffer;
    private ReadWriteBuffer<Ray> shadowRayBuffer;
    private ReadWriteBuffer<GeometryCollision> shadowCastBuffer;
    private IReadWriteNormalizedTexture2D<float4> luminanceBuffer;
#nullable restore
    
    /// <summary>
    /// Initializes a new instance of the <see cref="GlossyShader"/> struct.
    /// </summary>
    /// <param name="matId">The material id associated to the shader.</param>
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

    int IMaterialShader<GlossyMaterial>.MaterialId { set => matId = value; }

    GlossyMaterial IMaterialShader<GlossyMaterial>.Material { set => material = value; }

    ReadOnlyBuffer<ObjectSpace> IMaterialShader<GlossyMaterial>.ObjectBuffer  { set => objectBuffer = value; }

    ReadOnlyBuffer<Light> IMaterialShader<GlossyMaterial>.LightBuffer  { set => lightBuffer = value; }

    ReadWriteBuffer<Ray> IMaterialShader<GlossyMaterial>.PathRayBuffer { set => pathRayBuffer = value; }

    ReadWriteBuffer<GeometryCollision> IMaterialShader<GlossyMaterial>.PathCastBuffer {  set => pathCastBuffer = value; }

    ReadWriteBuffer<Ray> IMaterialShader<GlossyMaterial>.ShadowRayBuffer { set => shadowRayBuffer = value; }

    ReadWriteBuffer<GeometryCollision> IMaterialShader<GlossyMaterial>.ShadowCastBuffer { set => shadowCastBuffer = value; }

    IReadWriteNormalizedTexture2D<float4> IMaterialShader<GlossyMaterial>.AttenuationBuffer { set => _ = value; }

    IReadWriteNormalizedTexture2D<float4> IMaterialShader<GlossyMaterial>.LuminanceBuffer { set => luminanceBuffer = value; }
}
