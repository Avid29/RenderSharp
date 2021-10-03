using ComputeSharp;
using RenderSharp.RayTracing.HLSL.Rays;
using RenderSharp.RayTracing.HLSL.Utils;
using System;

namespace RenderSharp.RayTracing.HLSL.Components
{
    public struct FullCamera
    {
        public Float3 origin;
        public Float3 horizontal;
        public Float3 vertical;
        public Float3 lowerLeftCorner;

        public static FullCamera Create(Camera specs, float aspectRatio)
        {
            float theta = FloatUtils.DegreesToRadians(specs.fov);
            float h = MathF.Tan(theta/2);
            float height = 2 * h;
            float width = aspectRatio * height;

            Float3 vup = Float3.UnitY;
            Float3 w = Hlsl.Normalize(specs.origin - specs.look);
            Float3 u = Hlsl.Normalize(Hlsl.Cross(vup, w));
            Float3 v = Hlsl.Cross(w, u);

            FullCamera camera;
            camera.origin = specs.origin;
            camera.horizontal = width * u;
            camera.vertical = height * v;
            Float3 depth = w * specs.focalLength;

            camera.lowerLeftCorner = camera.origin - camera.horizontal / 2 - camera.vertical / 2 - depth;
            return camera;
        }

        public static Ray CreateRay(FullCamera camera, float u, float v)
        {
            Ray ray;
            ray.origin = camera.origin;
            ray.direction = camera.lowerLeftCorner + u * camera.horizontal + v * camera.vertical - camera.origin;
            return ray;
        }
    }
}
