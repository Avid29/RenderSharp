// Adam Dernis 2023

namespace RenderSharp.RayTracing.Models.Rays;

public struct RayCast
{
    public float3 position;
    public float3 normal;
    public float distance;
    public int objId;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="RayCast"/> struct.
    /// </summary>
    public RayCast(float3 position, float3 normal, float distance)
    {
        this.position = position;
        this.normal = normal;
        this.distance = distance;
        this.objId = -1;
    }
    
    /// <remarks>
    /// TODO: Replace with constructor
    /// Waiting on https://github.com/Sergio0694/ComputeSharp/issues/481
    /// </remarks>
    public static RayCast Create(float3 position, float3 normal, float distance)
    {
        RayCast cast;
        cast.position = position;
        cast.normal = normal;
        cast.distance = distance;
        cast.objId = -1;
        return cast;
    }
}
