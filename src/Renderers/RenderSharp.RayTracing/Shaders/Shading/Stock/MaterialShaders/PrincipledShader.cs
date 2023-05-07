// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Models.Geometry;
using RenderSharp.RayTracing.Models.Lighting;
using RenderSharp.RayTracing.Models.Materials;
using RenderSharp.RayTracing.RayCasts;
using RenderSharp.RayTracing.Shaders.Shading.Interfaces;

namespace RenderSharp.RayTracing.Shaders.Shading.Stock.MaterialShaders;

/// <summary>
/// A shader for a principle material.
/// </summary>
[EmbeddedBytecode(DispatchAxis.XY)]
public partial struct PrincipledShader : IMaterialShader<PrincipledMaterial>
{
#nullable disable
    private int matId;
    private PrincipledMaterial material;
    private ReadOnlyBuffer<ObjectSpace> objectBuffer;
    private ReadOnlyBuffer<Light> lightBuffer;
    private ReadWriteBuffer<Ray> rayBuffer;
    private ReadWriteBuffer<GeometryCollision> rayCastBuffer;
    private ReadWriteBuffer<Ray> shadowRayBuffer;
    private ReadWriteBuffer<GeometryCollision> shadowCastBuffer;
    private IReadWriteNormalizedTexture2D<float4> attenuationBuffer;
    private IReadWriteNormalizedTexture2D<float4> luminanceBuffer;
#nullable restore
    
    /// <summary>
    /// Initializes a new instance of the <see cref="PrincipledShader"/> struct.
    /// </summary>
    /// <param name="matId">The material id associated to the shader.</param>
    /// <param name="material">The material properties assigned to the shader instance.</param>
    public PrincipledShader(int matId, PrincipledMaterial material)
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
        
        var cast = rayCastBuffer[fIndex];
        var ray = rayBuffer[fIndex];

        // If this material was not hit do not execute
        if (cast.matId != matId)
            return;

        var n = cast.smoothNormal;
        var v = ray.direction;
        var r = Hlsl.Reflect(v, n);

        // Calculate diffuse and specular intensity
        float4 diffuseIntensity = float4.Zero;
        float4 glossyIntensity = float4.Zero;
        float4 specularIntensity = float4.Zero;
        for (int i = 0; i < lightBuffer.Length; i++)
        {
            var fShadowIndex = (i * DispatchSize.X * DispatchSize.Y) + (index2D.Y * DispatchSize.X) + index2D.X; 
            
            if (shadowCastBuffer[fShadowIndex].geoId != -1)
                continue;

            var l = shadowRayBuffer[fShadowIndex].direction;
            
            var lr = Hlsl.Reflect(l, cast.smoothNormal);
            
            diffuseIntensity += lightBuffer[i].radiance * Hlsl.Max(Hlsl.Dot(n, l), 0f);
            specularIntensity += lightBuffer[i].radiance * Hlsl.Pow(Hlsl.Max(Hlsl.Dot(lr, ray.direction), 0), material.roughness);
        }

        var att = attenuationBuffer[index2D];

        // Sum ambient, diffuse, and specular components
        luminanceBuffer[index2D] += material.ambient;
        luminanceBuffer[index2D] += material.diffuse * diffuseIntensity;
        luminanceBuffer[index2D] += material.specular * specularIntensity;

        if (material.transmission > material.metallic)
        {
            var refrN = n;
            var ior = material.ior;

            if (cast.isBackFace)
            {
                refrN = -refrN;
            }
            else
            {
                ior = 1 / ior;
            }

            var refr = Hlsl.Refract(v, refrN, ior);
            
            // Cannot refract
            if (Hlsl.Length(refr) == 0)
                refr = Hlsl.Reflect(ray.direction, cast.smoothNormal);

            rayBuffer[fIndex] = Ray.Create(cast.position, refr);
            attenuationBuffer[index2D] *= material.transmission;
        }
        else
        {
            rayBuffer[fIndex] = Ray.Create(cast.position, r);
            attenuationBuffer[index2D] *= material.metallic;
        }
    }
    
    int IMaterialShader<PrincipledMaterial>.MaterialId { set => matId = value; }

    PrincipledMaterial IMaterialShader<PrincipledMaterial>.Material { set => material = value; }

    ReadOnlyBuffer<ObjectSpace> IMaterialShader<PrincipledMaterial>.ObjectBuffer  { set => objectBuffer = value; }

    ReadOnlyBuffer<Light> IMaterialShader<PrincipledMaterial>.LightBuffer  { set => lightBuffer = value; }

    ReadWriteBuffer<Ray> IMaterialShader<PrincipledMaterial>.PathRayBuffer { set => rayBuffer = value; }

    ReadWriteBuffer<GeometryCollision> IMaterialShader<PrincipledMaterial>.PathCastBuffer {  set => rayCastBuffer = value; }

    ReadWriteBuffer<Ray> IMaterialShader<PrincipledMaterial>.ShadowRayBuffer { set => shadowRayBuffer = value; }

    ReadWriteBuffer<GeometryCollision> IMaterialShader<PrincipledMaterial>.ShadowCastBuffer { set => shadowCastBuffer = value; }

    IReadWriteNormalizedTexture2D<float4> IMaterialShader<PrincipledMaterial>.AttenuationBuffer { set => attenuationBuffer = value; }

    IReadWriteNormalizedTexture2D<float4> IMaterialShader<PrincipledMaterial>.LuminanceBuffer { set => luminanceBuffer = value; }
}
