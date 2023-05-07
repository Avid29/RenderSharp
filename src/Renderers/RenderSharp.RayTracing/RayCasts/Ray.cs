// Adam Dernis 2023

using System.Numerics;

namespace RenderSharp.RayTracing.RayCasts;

/// <summary>
/// A ray with an origin and a direction.
/// </summary>
public struct Ray
{
    /// <summary>
    /// The ray's origin.
    /// </summary>
    public Vector3 origin;
    
    /// <summary>
    /// The ray's direction.
    /// </summary>
    public Vector3 direction;

    /// <summary>
    /// Initializes a new instance of the <see cref="Ray"/> struct.
    /// </summary>
    public Ray(Vector3 origin, Vector3 direction)
    {
        this.origin = origin;
        this.direction = direction;
    }

    /// <remarks>
    /// TODO: Replace with constructor
    /// Waiting on https://github.com/Sergio0694/ComputeSharp/issues/481
    /// </remarks>
    public static Ray Create(Vector3 origin, Vector3 direction)
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
    public static Vector3 PointAt(Ray ray, float c)
        => ray.origin + c * ray.direction;
}
