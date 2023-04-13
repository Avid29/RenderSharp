// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Models.Geometry;
using RenderSharp.RayTracing.Models.Lighting;
using RenderSharp.RayTracing.Models.Materials;
using RenderSharp.RayTracing.Models.Rays;
using RenderSharp.RayTracing.Shaders.Shading.Interfaces;

namespace RenderSharp.RayTracing.Shaders.Shading.Stock.MaterialShaders;

[EmbeddedBytecode(DispatchAxis.XY)]
public partial struct PrincipledShader : IMaterialShader
{
    private readonly int matId;
    private readonly PrincipledMaterial material;
    
#nullable disable
    private ReadOnlyBuffer<ObjectSpace> objectBuffer;
    private ReadOnlyBuffer<Light> lightBuffer;
    private ReadWriteBuffer<Ray> rayBuffer;
    private ReadWriteBuffer<Ray> shadowCastBuffer;
    private ReadWriteBuffer<GeometryCollision> rayCastBuffer;
    private IReadWriteNormalizedTexture2D<float4> attenuationBuffer;
    private IReadWriteNormalizedTexture2D<float4> colorBuffer;
#nullable restore

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
            var l = shadowCastBuffer[fShadowIndex].direction;
            if (Hlsl.Length(l) == 0)
                continue;
            
            var lr = Hlsl.Reflect(l, cast.smoothNormal);
            
            diffuseIntensity += lightBuffer[i].color * Hlsl.Max(Hlsl.Dot(n, l), 0f);
            specularIntensity += lightBuffer[i].color * Hlsl.Pow(Hlsl.Max(Hlsl.Dot(lr, ray.direction), 0), material.roughness);
        }

        var att = attenuationBuffer[index2D];

        // Sum ambient, diffuse, and specular components
        colorBuffer[index2D] += material.ambient;
        colorBuffer[index2D] += material.diffuse * diffuseIntensity;
        colorBuffer[index2D] += material.specular * specularIntensity;

        if (material.transmission > material.metallic)
        {
            n *= cast.isBackFace ? -1 : 1;
            var refr = Hlsl.Refract(v, n, material.ior);
            rayBuffer[fIndex] = Ray.Create(cast.position, refr);
            attenuationBuffer[index2D] *= material.transmission;
        }
        else
        {
            rayBuffer[fIndex] = Ray.Create(cast.position, r);
            attenuationBuffer[index2D] *= material.metallic;
        }
    }

    ReadOnlyBuffer<ObjectSpace> IMaterialShader.ObjectBuffer  { set => objectBuffer = value; }

    ReadOnlyBuffer<Light> IMaterialShader.LightBuffer  { set => lightBuffer = value; }

    ReadWriteBuffer<Ray> IMaterialShader.RayBuffer { set => rayBuffer = value; }

    ReadWriteBuffer<Ray> IMaterialShader.ShadowCastBuffer { set => shadowCastBuffer = value; }

    ReadWriteBuffer<GeometryCollision> IMaterialShader.RayCastBuffer {  set => rayCastBuffer = value; }

    IReadWriteNormalizedTexture2D<float4> IMaterialShader.AttenuationBuffer { set => attenuationBuffer = value; }

    IReadWriteNormalizedTexture2D<float4> IMaterialShader.ColorBuffer { set => colorBuffer = value; }
}
