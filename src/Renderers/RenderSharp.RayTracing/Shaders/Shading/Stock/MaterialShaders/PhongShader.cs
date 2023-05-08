// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Models.Geometry;
using RenderSharp.RayTracing.Models.Lighting;
using RenderSharp.RayTracing.Models.Materials;
using RenderSharp.RayTracing.RayCasts;
using RenderSharp.RayTracing.Shaders.Shading.Interfaces;

namespace RenderSharp.RayTracing.Shaders.Shading.Stock.MaterialShaders;

/// <summary>
/// A shader for a phong material.
/// </summary>
[EmbeddedBytecode(DispatchAxis.XY)]
public partial struct PhongShader : IMaterialShader<PhongMaterial>
{
#nullable disable
    private int matId;
    private PhongMaterial material;
    private ReadOnlyBuffer<ObjectSpace> objectBuffer;
    private ReadOnlyBuffer<Light> lightBuffer;
    private ReadWriteBuffer<Ray> pathRayBuffer;
    private ReadWriteBuffer<GeometryCollision> pathCastBuffer;
    private ReadWriteBuffer<Ray> shadowRayBuffer;
    private ReadWriteBuffer<GeometryCollision> shadowCastBuffer;
    private IReadWriteNormalizedTexture2D<float4> attenuationBuffer;
    private IReadWriteNormalizedTexture2D<float4> luminanceBuffer;
#nullable restore
    
    /// <summary>
    /// Initializes a new instance of the <see cref="PhongShader"/> struct.
    /// </summary>
    /// <param name="matId">The material id associated to the shader.</param>
    /// <param name="material">The material properties assigned to the shader instance.</param>
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
            
            diffuseIntensity += lightBuffer[i].radiance * Hlsl.Max(Hlsl.Dot(n, l), 0f);
            specularIntensity += lightBuffer[i].radiance * Hlsl.Pow(Hlsl.Max(Hlsl.Dot(r, ray.direction), 0), material.roughness);
        }

        var att = attenuationBuffer[index2D];

        // Sum ambient, diffuse, and specular components
        luminanceBuffer[index2D] += material.ambient;
        luminanceBuffer[index2D] += material.diffuse * diffuseIntensity;
        luminanceBuffer[index2D] += material.specular * specularIntensity;
        attenuationBuffer[index2D] = 0;
        pathRayBuffer[fIndex] = default;
    }
    
    int IMaterialShader<PhongMaterial>.MaterialId { set => matId = value; }

    PhongMaterial IMaterialShader<PhongMaterial>.Material { set => material = value; }

    ReadOnlyBuffer<ObjectSpace> IMaterialShader<PhongMaterial>.ObjectBuffer  { set => objectBuffer = value; }

    ReadOnlyBuffer<Light> IMaterialShader<PhongMaterial>.LightBuffer  { set => lightBuffer = value; }

    ReadWriteBuffer<Ray> IMaterialShader<PhongMaterial>.PathRayBuffer { set => pathRayBuffer = value; }

    ReadWriteBuffer<GeometryCollision> IMaterialShader<PhongMaterial>.PathCastBuffer {  set => pathCastBuffer = value; }

    ReadWriteBuffer<Ray> IMaterialShader<PhongMaterial>.ShadowRayBuffer { set => shadowRayBuffer = value; }

    ReadWriteBuffer<GeometryCollision> IMaterialShader<PhongMaterial>.ShadowCastBuffer { set => shadowCastBuffer = value; }

    IReadWriteNormalizedTexture2D<float4> IMaterialShader<PhongMaterial>.AttenuationBuffer { set => attenuationBuffer = value; }

    IReadWriteNormalizedTexture2D<float4> IMaterialShader<PhongMaterial>.LuminanceBuffer { set => luminanceBuffer = value; }
}
