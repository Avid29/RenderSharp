// Adam Dernis 2023

namespace RenderSharp.RayTracing.Models.Rays;

public struct RayCast
{
    public float3 position;
    public float3 normal;
    public float distance;

    public RayCast(float3 position, float3 normal, float distance)
    {
        this.position = position;
        this.normal = normal;
        this.distance = distance;
    }
}
