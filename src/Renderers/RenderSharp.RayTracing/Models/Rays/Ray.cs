// Adam Dernis 2023

namespace RenderSharp.RayTracing.Models.Rays;

public struct Ray
{
    public float3 origin;
    public float3 direction;

    /// <remarks>
    /// Wishful thinking.
    /// </remarks>
    public Ray(float3 origin, float3 direction)
    {
        this.origin = origin;
        this.direction = direction;
    }

    /// <remarks>
    /// Because ComputeSharp STILL doesn't support contructors.
    /// </remarks>
    public static Ray Create(float3 origin, float3 direction)
    {
        Ray ray;
        ray.origin = origin;
        ray.direction = direction;
        return ray;
    }
}
