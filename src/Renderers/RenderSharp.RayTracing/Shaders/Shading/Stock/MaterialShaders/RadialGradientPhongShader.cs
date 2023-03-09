// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Models.Lighting;
using RenderSharp.RayTracing.Models.Materials;
using RenderSharp.RayTracing.Models.Rays;
using RenderSharp.RayTracing.Shaders.Shading.Interfaces;

namespace RenderSharp.RayTracing.Shaders.Shading.Stock.MaterialShaders;

[EmbeddedBytecode(DispatchAxis.XY)]
public partial struct RadialGradientPhongShader : IMaterialShader
{
    private readonly int matId;
    private readonly RadialGradientPhongMaterial material;
    
#nullable disable
    private ReadOnlyBuffer<Light> lightsBuffer;
    private ReadWriteBuffer<Ray> rayBuffer;
    private ReadWriteBuffer<Ray> shadowCastBuffer;
    private ReadWriteBuffer<GeometryCollision> rayCastBuffer;
    private IReadWriteNormalizedTexture2D<float4> colorBuffer;
#nullable restore
    
    public RadialGradientPhongShader(int matId, RadialGradientPhongMaterial material)
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

        // Calculate diffuse and specular intensity
        float4 diffuseIntensity = float4.Zero;
        float4 specularIntensity = float4.Zero;
        for (int i = 0; i < lightsBuffer.Length; i++)
        {
            var fShadowIndex = (i * DispatchSize.X * DispatchSize.Y) + (index2D.Y * DispatchSize.X) + index2D.X; 
            var l = shadowCastBuffer[fShadowIndex].direction;
            if (Hlsl.Length(l) == 0)
                continue;

            var n = cast.smoothNormal;
            var v = ray.direction;
            var h = Hlsl.Normalize(l - v);

            diffuseIntensity += lightsBuffer[i].color * Hlsl.Max(Hlsl.Dot(n, l), 0f);
            specularIntensity += lightsBuffer[i].color * Hlsl.Pow(Hlsl.Dot(n, h), material.roughness);
        }

        var x = Hlsl.Clamp(Hlsl.Length(cast.position) / material.scale, 0f, 1f);
        var diffuse = (material.diffuse0 * x) + (material.diffuse1 * (1 - x));

        // Sum ambient, diffuse, and specular components
        colorBuffer[index2D] += material.ambient;
        colorBuffer[index2D] += diffuse * diffuseIntensity;
        colorBuffer[index2D] += material.specular * specularIntensity;
    }

    ReadOnlyBuffer<Light> IMaterialShader.LightBuffer  { set => lightsBuffer = value; }

    ReadWriteBuffer<Ray> IMaterialShader.RayBuffer { set => rayBuffer = value; }

    ReadWriteBuffer<Ray> IMaterialShader.ShadowCastBuffer { set => shadowCastBuffer = value; }

    ReadWriteBuffer<GeometryCollision> IMaterialShader.RayCastBuffer {  set => rayCastBuffer = value; }

    IReadWriteNormalizedTexture2D<float4> IMaterialShader.AttenuationBuffer { set => _ = value; }

    IReadWriteNormalizedTexture2D<float4> IMaterialShader.ColorBuffer { set => colorBuffer = value; }
}
