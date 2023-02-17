// Adam Dernis 2023

namespace RenderSharp.RayTracing.Models.Rays;

/// <summary>
/// A ray with an origin and a direction.
/// </summary>
public struct Ray
{
    public float3 origin;
    public float3 direction;

    /// <summary>
    /// Initializes a new instance of the <see cref="Ray"/> struct.
    /// </summary>
    public Ray(float3 origin, float3 direction)
    {
        this.origin = origin;
        this.direction = direction;
    }

    /// <remarks>
    /// TODO: Replace with constructor
    /// Waiting on https://github.com/Sergio0694/ComputeSharp/issues/481
    /// </remarks>
    public static Ray Create(float3 origin, float3 direction)
    {
        Ray ray;
        ray.origin = origin;
        ray.direction = direction;
        return ray;
    }
    
    /// <remarks>
    /// TODO: Convert to an instance method.
    /// Waiting on https://github.com/Sergio0694/ComputeSharp/issues/479
    /// </remarks>
    public static float3 PointAt(Ray ray, float c)
        => ray.origin + c * ray.direction;
}
