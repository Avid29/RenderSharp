using ComputeSharp;
using RenderSharp.RayTracing.HLSL.Scenes.Rays;
using System.Numerics;

namespace RenderSharp.RayTracing.HLSL.Scenes.Skys
{
    public struct Sky
    {
        public Float4 color;

        public static Float4 Color(Sky sky, RayCast ray)
        {
            Float3 unitDirection = Vector3.Normalize(ray.normal);
            float t = 0.5f * (unitDirection.Y + 1);
            return (1f - t) * Float4.One + t * sky.color;
        }
    }
}
