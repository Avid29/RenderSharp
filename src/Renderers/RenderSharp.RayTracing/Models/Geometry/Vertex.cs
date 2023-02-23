// Adam Dernis 2023

namespace RenderSharp.RayTracing.Models.Geometry;

public struct Vertex
{
    public float3 position;
    public float3 normal;
    //public float2 texturePos;

    public Vertex(float3 position, float3 normal)
    {
        this.position = position;
        this.normal = normal;
    }
}
