// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Models.Geometry;
using RenderSharp.RayTracing.Models.Lighting;
using RenderSharp.RayTracing.RayCasts;

namespace RenderSharp.RayTracing.Shaders.Shading.Interfaces;

public interface IMaterialShader : IComputeShader
{
    ReadOnlyBuffer<ObjectSpace> ObjectBuffer { set; }

    ReadOnlyBuffer<Light> LightBuffer { set; }

    ReadWriteBuffer<Ray> PathRayBuffer { set; }

    ReadWriteBuffer<GeometryCollision> PathCastBuffer { set; }

    ReadWriteBuffer<Ray> ShadowRayBuffer { set; }

    ReadWriteBuffer<GeometryCollision> ShadowCastBuffer { set; }

    IReadWriteNormalizedTexture2D<float4> AttenuationBuffer { set; }

    IReadWriteNormalizedTexture2D<float4> ColorBuffer { set; }
}
