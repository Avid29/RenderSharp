// Adam Dernis 2023

using System.Numerics;

namespace RenderSharp.RayTracing.Models.Lighting;

public struct Light
{
    public float3 position;
    public float4 color;
    public float radius;

    public Light(float3 position, Vector3 color, float power, float radius)
    {
        this.position = position;
        this.color = new Vector4(color * power, 1);
        this.radius = radius;
    }
}
