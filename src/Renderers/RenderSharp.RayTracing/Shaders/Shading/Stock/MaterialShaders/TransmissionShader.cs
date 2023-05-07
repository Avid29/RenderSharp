// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Models.Geometry;
using RenderSharp.RayTracing.Models.Lighting;
using RenderSharp.RayTracing.Models.Materials;
using RenderSharp.RayTracing.RayCasts;
using RenderSharp.RayTracing.Shaders.Shading.Interfaces;

namespace RenderSharp.RayTracing.Shaders.Shading.Stock.MaterialShaders;

/// <summary>
/// A shader for a transmissive material.
/// </summary>
[EmbeddedBytecode(DispatchAxis.XY)]
public partial struct TransmissionShader : IMaterialShader<TransmissionMaterial>
{
#nullable disable
    private int matId;
    private TransmissionMaterial material;
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
    /// Initializes a new instance of the <see cref="TransmissionShader"/> struct.
    /// </summary>
    /// <param name="matId">The material id associated to the shader.</param>
    /// <param name="material">The material properties assigned to the shader instance.</param>
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
        
        var cast = pathCastBuffer[fIndex];
        var ray = pathRayBuffer[fIndex];

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

        pathRayBuffer[fIndex] = Ray.Create(cast.position, r);
    }
    
    int IMaterialShader<TransmissionMaterial>.MaterialId { set => matId = value; }

    TransmissionMaterial IMaterialShader<TransmissionMaterial>.Material { set => material = value; }

    ReadOnlyBuffer<ObjectSpace> IMaterialShader<TransmissionMaterial>.ObjectBuffer  { set => objectBuffer = value; }

    ReadOnlyBuffer<Light> IMaterialShader<TransmissionMaterial>.LightBuffer  { set => lightBuffer = value; }

    ReadWriteBuffer<Ray> IMaterialShader<TransmissionMaterial>.PathRayBuffer { set => pathRayBuffer = value; }

    ReadWriteBuffer<GeometryCollision> IMaterialShader<TransmissionMaterial>.PathCastBuffer {  set => pathCastBuffer = value; }

    ReadWriteBuffer<Ray> IMaterialShader<TransmissionMaterial>.ShadowRayBuffer { set => shadowRayBuffer = value; }

    ReadWriteBuffer<GeometryCollision> IMaterialShader<TransmissionMaterial>.ShadowCastBuffer { set => shadowCastBuffer = value; }

    IReadWriteNormalizedTexture2D<float4> IMaterialShader<TransmissionMaterial>.AttenuationBuffer { set => attenuationBuffer = value; }

    IReadWriteNormalizedTexture2D<float4> IMaterialShader<TransmissionMaterial>.LuminanceBuffer { set => luminanceBuffer = value; }
}
