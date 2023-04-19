// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Models.Geometry;
using RenderSharp.RayTracing.Models.Lighting;
using RenderSharp.RayTracing.RayCasts;

namespace RenderSharp.RayTracing.Shaders.Shading.Interfaces;

/// <summary>
/// An interface for material shaders.
/// </summary>
public interface IMaterialShader : IComputeShader
{
    /// <summary>
    /// Sets the scene object buffer on the shader.
    /// </summary>
    ReadOnlyBuffer<ObjectSpace> ObjectBuffer { set; }
    
    /// <summary>
    /// Sets the scene object buffer on the shader.
    /// </summary>
    ReadOnlyBuffer<Light> LightBuffer { set; }
    
    /// <summary>
    /// Sets the tile path ray buffer on the shader.
    /// </summary>
    ReadWriteBuffer<Ray> PathRayBuffer { set; }
    
    /// <summary>
    /// Sets the tile path cast buffer on the shader.
    /// </summary>
    ReadWriteBuffer<GeometryCollision> PathCastBuffer { set; }
    
    /// <summary>
    /// Sets the tile shadow ray buffer on the shader.
    /// </summary>
    ReadWriteBuffer<Ray> ShadowRayBuffer { set; }
    
    /// <summary>
    /// Sets the tile shadow cast buffer on the shader.
    /// </summary>
    ReadWriteBuffer<GeometryCollision> ShadowCastBuffer { set; }
    
    /// <summary>
    /// Sets the tile attenuation buffer on the shader.
    /// </summary>
    IReadWriteNormalizedTexture2D<float4> AttenuationBuffer { set; }
    
    /// <summary>
    /// Sets the tile luminance buffer on the shader.
    /// </summary>
    IReadWriteNormalizedTexture2D<float4> LuminanceBuffer { set; }
}
