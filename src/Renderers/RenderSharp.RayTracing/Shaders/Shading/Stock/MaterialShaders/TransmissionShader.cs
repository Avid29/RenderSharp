// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Models.Geometry;
using RenderSharp.RayTracing.Models.Lighting;
using RenderSharp.RayTracing.Models.Materials;
using RenderSharp.RayTracing.Models.Rays;
using RenderSharp.RayTracing.Shaders.Shading.Interfaces;

namespace RenderSharp.RayTracing.Shaders.Shading.Stock.MaterialShaders;

[EmbeddedBytecode(DispatchAxis.XY)]
public partial struct TransmissionShader : IMaterialShader
{
    private readonly int matId;
    private readonly TransmissionMaterial material;
    
#nullable disable
    private ReadOnlyBuffer<ObjectSpace> objectBuffer;
    private ReadOnlyBuffer<Light> lightBuffer;
    private ReadWriteBuffer<Ray> rayBuffer;
    private ReadWriteBuffer<Ray> shadowCastBuffer;
    private ReadWriteBuffer<GeometryCollision> rayCastBuffer;
    private IReadWriteNormalizedTexture2D<float4> attenuationBuffer;
    private IReadWriteNormalizedTexture2D<float4> colorBuffer;
#nullable restore
    
    public TransmissionShader(int matId, TransmissionMaterial material)
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
        var ior = material.ior;

        if (cast.isBackFace)
        {
            n *= -1;
            ior = 1 - (1 - ior);
        }

        var r = Hlsl.Refract(ray.direction, n, ior);

        // Cannot refract
        if (Hlsl.Length(r) == 0)
            r = Hlsl.Reflect(ray.direction, cast.smoothNormal);

        rayBuffer[fIndex] = Ray.Create(cast.position, r);
    }


    ReadOnlyBuffer<ObjectSpace> IMaterialShader.ObjectBuffer  { set => objectBuffer = value; }

    ReadOnlyBuffer<Light> IMaterialShader.LightBuffer  { set => lightBuffer = value; }

    ReadWriteBuffer<Ray> IMaterialShader.RayBuffer { set => rayBuffer = value; }

    ReadWriteBuffer<Ray> IMaterialShader.ShadowCastBuffer { set => shadowCastBuffer = value; }

    ReadWriteBuffer<GeometryCollision> IMaterialShader.RayCastBuffer {  set => rayCastBuffer = value; }

    IReadWriteNormalizedTexture2D<float4> IMaterialShader.AttenuationBuffer { set => attenuationBuffer = value; }

    IReadWriteNormalizedTexture2D<float4> IMaterialShader.ColorBuffer { set => colorBuffer = value; }
}
