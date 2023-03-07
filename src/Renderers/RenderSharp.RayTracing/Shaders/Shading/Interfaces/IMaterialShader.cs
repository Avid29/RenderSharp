// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Models.Lighting;
using RenderSharp.RayTracing.Models.Rays;

namespace RenderSharp.RayTracing.Shaders.Shading.Interfaces;

public interface IMaterialShader : IComputeShader
{
    ReadOnlyBuffer<Light> LightBuffer { set; }

    ReadWriteBuffer<Ray> RayBuffer { set; }

    ReadWriteBuffer<Ray> ShadowCastBuffer { set; }

    ReadWriteBuffer<GeometryCollision> RayCastBuffer { set; }

    IReadWriteNormalizedTexture2D<float4> AttenuationBuffer { set; }

    IReadWriteNormalizedTexture2D<float4> ColorBuffer { set; }
}
