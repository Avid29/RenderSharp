using RenderSharp.RayTracing.CPU.Scenes.Rays;
using System.Numerics;

namespace RenderSharp.RayTracing.CPU.Scenes.Skys
{
    public struct Sky
    {
        public Sky(Vector4 color)
        {
            Color = color;
        }

        public Vector4 Color { get; }

        public Vector4 GetColor(Ray ray)
        {
            Vector3 unitDirection = Vector3.Normalize(ray.Direction);
            float t = 0.5f * (unitDirection.Y + 1);
            return (1f - t) * Vector4.One + t * Color;
        }
    }
}
