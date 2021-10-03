using ComputeSharp;
using RenderSharp.RayTracing.HLSL.Rays;

namespace RenderSharp.RayTracing.HLSL.Components
{
    public struct Camera
    {
        public Float3 origin;
        public Float3 horizontal;
        public Float3 vertical;
        public Float3 lowerLeftCorner;

        public static Camera CreateCamera(Float3 origin, float width, float height, float focalLength)
        {
            Camera camera;
            camera.origin = origin;
            camera.horizontal = Float3.UnitX * width;
            camera.vertical = Float3.UnitY * height;
            Float3 depth = Float3.UnitZ * focalLength;
            camera.lowerLeftCorner = camera.origin - camera.horizontal / 2 - camera.vertical / 2 - depth;
            return camera;
        }

        public static Ray CreateRay(Camera camera, float u, float v)
        {
            Ray ray;
            ray.origin = camera.origin;
            ray.direction = camera.lowerLeftCorner + u * camera.horizontal + v * camera.vertical - camera.origin;
            return ray;
        }
    }
}
