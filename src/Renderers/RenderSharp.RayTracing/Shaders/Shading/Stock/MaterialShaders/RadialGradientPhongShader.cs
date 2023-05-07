// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Models.Geometry;
using RenderSharp.RayTracing.Models.Lighting;
using RenderSharp.RayTracing.Models.Materials;
using RenderSharp.RayTracing.RayCasts;
using RenderSharp.RayTracing.Shaders.Shading.Interfaces;

namespace RenderSharp.RayTracing.Shaders.Shading.Stock.MaterialShaders;

/// <summary>
/// A shader for a phong material with a radial gradient texture pattern.
/// </summary>
[EmbeddedBytecode(DispatchAxis.XY)]
public partial struct RadialGradientPhongShader : IMaterialShader<RadialGradientPhongMaterial>
{
#nullable disable
    private int matId;
    private RadialGradientPhongMaterial material;
    private ReadOnlyBuffer<ObjectSpace> objectBuffer;
    private ReadOnlyBuffer<Light> lightBuffer;
    private ReadWriteBuffer<Ray> pathRayBuffer;
    private ReadWriteBuffer<GeometryCollision> pathCastBuffer;
    private ReadWriteBuffer<Ray> shadowRayBuffer;
    private ReadWriteBuffer<GeometryCollision> shadowCastBuffer;
    private IReadWriteNormalizedTexture2D<float4> luminanceBuffer;
#nullable restore
    
    /// <summary>
    /// Initializes a new instance of the <see cref="RadialGradientPhongShader"/> struct.
    /// </summary>
    /// <param name="matId">The material id associated to the shader.</param>
    /// <param name="material">The material properties assigned to the shader instance.</param>
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
            var h = Hlsl.Normalize(l - v);

            diffuseIntensity += lightBuffer[i].radiance * Hlsl.Max(Hlsl.Dot(n, l), 0f);
            specularIntensity += lightBuffer[i].radiance * Hlsl.Pow(Hlsl.Dot(n, h), material.roughness);
        }

        var pos = cast.position;

        // Object space
        if (material.textureSpace == 1)
        {
            var objectSpace = objectBuffer[cast.objId];
            pos = Hlsl.Mul(new float4(pos, 1), objectSpace.inverseTransformation).XYZ;
        }

        var x = Hlsl.Clamp(Hlsl.Length(pos) / material.scale, 0f, 1f);
        //var diffuse = new float4(VectorUtils.HSVtoRGB(new float3(x * 360f, 1f, 1f)), 1);
        var diffuse = (material.diffuse0 * x) + (material.diffuse1 * (1 - x));

        // Sum ambient, diffuse, and specular components
        luminanceBuffer[index2D] += diffuse * material.cAmbient;
        luminanceBuffer[index2D] += diffuse * diffuseIntensity;
        luminanceBuffer[index2D] += material.specular * specularIntensity;
    }
    
    int IMaterialShader<RadialGradientPhongMaterial>.MaterialId { set => matId = value; }

    RadialGradientPhongMaterial IMaterialShader<RadialGradientPhongMaterial>.Material { set => material = value; }

    ReadOnlyBuffer<ObjectSpace> IMaterialShader<RadialGradientPhongMaterial>.ObjectBuffer  { set => objectBuffer = value; }

    ReadOnlyBuffer<Light> IMaterialShader<RadialGradientPhongMaterial>.LightBuffer  { set => lightBuffer = value; }

    ReadWriteBuffer<Ray> IMaterialShader<RadialGradientPhongMaterial>.PathRayBuffer { set => pathRayBuffer = value; }

    ReadWriteBuffer<GeometryCollision> IMaterialShader<RadialGradientPhongMaterial>.PathCastBuffer {  set => pathCastBuffer = value; }

    ReadWriteBuffer<Ray> IMaterialShader<RadialGradientPhongMaterial>.ShadowRayBuffer { set => shadowRayBuffer = value; }

    ReadWriteBuffer<GeometryCollision> IMaterialShader<RadialGradientPhongMaterial>.ShadowCastBuffer { set => shadowCastBuffer = value; }

    IReadWriteNormalizedTexture2D<float4> IMaterialShader<RadialGradientPhongMaterial>.AttenuationBuffer { set => _ = value; }

    IReadWriteNormalizedTexture2D<float4> IMaterialShader<RadialGradientPhongMaterial>.LuminanceBuffer { set => luminanceBuffer = value; }
}
