// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Models.Geometry;
using RenderSharp.RayTracing.Models.Lighting;
using RenderSharp.RayTracing.Models.Materials;
using RenderSharp.RayTracing.RayCasts;
using RenderSharp.RayTracing.Shaders.Shading.Interfaces;

namespace RenderSharp.RayTracing.Shaders.Shading.Stock.MaterialShaders;

/// <summary>
/// A shader for a phong material with a voronoi texture pattern.
/// </summary>
[EmbeddedBytecode(DispatchAxis.XY)]
public partial struct VoronoiPhongShader : IMaterialShader<VoronoiPhongMaterial>
{
#nullable disable
    private int matId;
    private VoronoiPhongMaterial material;
    private ReadOnlyBuffer<ObjectSpace> objectBuffer;
    private ReadOnlyBuffer<Light> lightBuffer;
    private ReadWriteBuffer<Ray> pathRayBuffer;
    private ReadWriteBuffer<GeometryCollision> pathRayCastBuffer;
    private ReadWriteBuffer<Ray> shadowRayBuffer;
    private ReadWriteBuffer<GeometryCollision> shadowCastBuffer;
    private IReadWriteNormalizedTexture2D<float4> luminanceBuffer;
#nullable restore

    /// <summary>
    /// Initializes a new instance of the <see cref="VoronoiPhongShader"/> struct.
    /// </summary>
    /// <param name="matId">The material id associated to the shader.</param>
    public VoronoiPhongShader(int matId)
    {
        this.matId = matId;
    }

    /// <inheritdoc/>
    public void Execute()
    {
        float4 diffuse = float4.One;
        float4 specular = float4.One;
        float roughness = 80f;
        float cAmbient = 0.2f;

        var pos1 = new float3(-1.3f, 0, 2.3f);
        var pos2 = new float3(0.5f, 0, 2.5f);
        var pos3 = new float3(0.2f, 0.2f, 2.8f);
        var pos4 = new float3(-0.5f, 0, 4.5f);

        // Get the index of resources managed by the current thread
        // in both 2D textures and flat buffers
        int2 index2D = ThreadIds.XY;
        int fIndex = (index2D.Y * DispatchSize.X) + index2D.X;
        
        var cast = pathRayCastBuffer[fIndex];
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
            specularIntensity += lightBuffer[i].radiance * Hlsl.Pow(Hlsl.Max(Hlsl.Dot(r, ray.direction), 0), roughness);
        }

        // Evaluate texture
        var pos = cast.position;
        var dis1 = Hlsl.Length(pos1 - pos);
        var dis2 = Hlsl.Length(pos2 - pos);
        var dis3 = Hlsl.Length(pos3 - pos);
        var dis4 = Hlsl.Length(pos4 - pos);
        var minDistance = Hlsl.Min(Hlsl.Min(dis1, dis2), Hlsl.Min(dis3, dis4));
        //var minDistance = Hlsl.Min(Hlsl.Min(dis1, dis2), dis3);

        diffuse *= minDistance;

        //if (minDistance == dis1)
        //    diffuse = float4.UnitX;
        //else if (minDistance == dis2)
        //    diffuse = float4.UnitY;
        //else if (minDistance == dis3)
        //    diffuse = float4.UnitZ;
        //else if (minDistance == dis4)
        //    diffuse = float4.UnitX + float4.UnitY;

        //diffuse += float4.UnitW;

        // Sum ambient, diffuse, and specular components
        luminanceBuffer[index2D] += diffuse * cAmbient;
        luminanceBuffer[index2D] += diffuse * diffuseIntensity;
        luminanceBuffer[index2D] += specular * specularIntensity;
    }
    
    int IMaterialShader<VoronoiPhongMaterial>.MaterialId { set => matId = value; }

    VoronoiPhongMaterial IMaterialShader<VoronoiPhongMaterial>.Material { set => material = value; }

    ReadOnlyBuffer<ObjectSpace> IMaterialShader<VoronoiPhongMaterial>.ObjectBuffer  { set => objectBuffer = value; }

    ReadOnlyBuffer<Light> IMaterialShader<VoronoiPhongMaterial>.LightBuffer  { set => lightBuffer = value; }

    ReadWriteBuffer<Ray> IMaterialShader<VoronoiPhongMaterial>.PathRayBuffer { set => pathRayBuffer = value; }

    ReadWriteBuffer<GeometryCollision> IMaterialShader<VoronoiPhongMaterial>.PathCastBuffer {  set => pathRayCastBuffer = value; }

    ReadWriteBuffer<Ray> IMaterialShader<VoronoiPhongMaterial>.ShadowRayBuffer { set => shadowRayBuffer = value; }

    ReadWriteBuffer<GeometryCollision> IMaterialShader<VoronoiPhongMaterial>.ShadowCastBuffer { set => shadowCastBuffer = value; }

    IReadWriteNormalizedTexture2D<float4> IMaterialShader<VoronoiPhongMaterial>.AttenuationBuffer { set => _ = value; }

    IReadWriteNormalizedTexture2D<float4> IMaterialShader<VoronoiPhongMaterial>.LuminanceBuffer { set => luminanceBuffer = value; }
}
