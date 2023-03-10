// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Models.Geometry;
using RenderSharp.RayTracing.Models.Lighting;
using RenderSharp.RayTracing.Models.Materials;
using RenderSharp.RayTracing.Models.Rays;
using RenderSharp.RayTracing.Shaders.Shading.Interfaces;

namespace RenderSharp.RayTracing.Shaders.Shading.Stock.MaterialShaders;

[EmbeddedBytecode(DispatchAxis.XY)]
public partial struct VoronoiPhongShader : IMaterialShader
{
    private readonly int matId;
    
#nullable disable
    private ReadOnlyBuffer<ObjectSpace> objectBuffer;
    private ReadOnlyBuffer<Light> lightBuffer;
    private ReadWriteBuffer<Ray> rayBuffer;
    private ReadWriteBuffer<Ray> shadowCastBuffer;
    private ReadWriteBuffer<GeometryCollision> rayCastBuffer;
    private IReadWriteNormalizedTexture2D<float4> colorBuffer;
#nullable restore

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
        
        var cast = rayCastBuffer[fIndex];
        var ray = rayBuffer[fIndex];

        // If this material was not hit do not execute
        if (cast.matId != matId)
            return;

        // Calculate diffuse and specular intensity
        float4 diffuseIntensity = float4.Zero;
        float4 specularIntensity = float4.Zero;
        for (int i = 0; i < lightBuffer.Length; i++)
        {
            var fShadowIndex = (i * DispatchSize.X * DispatchSize.Y) + (index2D.Y * DispatchSize.X) + index2D.X; 
            var l = shadowCastBuffer[fShadowIndex].direction;
            if (Hlsl.Length(l) == 0)
                continue;
            
            var n = cast.smoothNormal;
            var v = ray.direction;
            var r = Hlsl.Reflect(l, cast.smoothNormal);
            
            diffuseIntensity += lightBuffer[i].color * Hlsl.Max(Hlsl.Dot(n, l), 0f);
            specularIntensity += lightBuffer[i].color * Hlsl.Pow(Hlsl.Max(Hlsl.Dot(r, ray.direction), 0), roughness);
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
        colorBuffer[index2D] += diffuse * cAmbient;
        colorBuffer[index2D] += diffuse * diffuseIntensity;
        colorBuffer[index2D] += specular * specularIntensity;
    }

    ReadOnlyBuffer<ObjectSpace> IMaterialShader.ObjectBuffer  { set => objectBuffer = value; }

    ReadOnlyBuffer<Light> IMaterialShader.LightBuffer  { set => lightBuffer = value; }

    ReadWriteBuffer<Ray> IMaterialShader.RayBuffer { set => rayBuffer = value; }

    ReadWriteBuffer<Ray> IMaterialShader.ShadowCastBuffer { set => shadowCastBuffer = value; }

    ReadWriteBuffer<GeometryCollision> IMaterialShader.RayCastBuffer {  set => rayCastBuffer = value; }

    IReadWriteNormalizedTexture2D<float4> IMaterialShader.AttenuationBuffer { set => _ = value; }

    IReadWriteNormalizedTexture2D<float4> IMaterialShader.ColorBuffer { set => colorBuffer = value; }
}
