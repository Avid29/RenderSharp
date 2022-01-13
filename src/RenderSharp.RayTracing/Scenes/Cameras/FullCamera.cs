using ComputeSharp;
using RenderSharp.RayTracing.Scenes.Rays;
using RenderSharp.RayTracing.Utils;
using System;

namespace RenderSharp.RayTracing.Scenes.Cameras
{
    public struct FullCamera
    {
        public float3 origin;
        public float3 horizontal;
        public float3 vertical;
        public float3 lowerLeftCorner;
        public float3 u, v, w;
        public float lensRadius;

        public static FullCamera Create(Camera specs, float aspectRatio)
        {
            float theta = FloatUtils.DegreesToRadians(specs.fov);
#if NET5_0_OR_GREATER
            float h = MathF.Tan(theta / 2);
#elif NETSTANDARD2_0_OR_GREATER
            float h = (float)Math.Tan(theta / 2);
#endif
            float height = 2 * h;
            float width = aspectRatio * height;

             float3 vup =  float3.UnitY;

            FullCamera camera;
            camera.origin = specs.origin;
            camera.w = Hlsl.Normalize(specs.origin - specs.look);
            camera.u = Hlsl.Normalize(Hlsl.Cross(vup, camera.w));
            camera.v = Hlsl.Cross(camera.w, camera.u);
            camera.horizontal = width * camera.u;
            camera.vertical = height * camera.v;
             float3 depth = camera.w * specs.focalLength;

            camera.lowerLeftCorner = camera.origin - camera.horizontal / 2 - camera.vertical / 2 - depth;

            camera.lensRadius = specs.aperture / 2;
            return camera;
        }

        public static Ray CreateRay(FullCamera camera, float u, float v, ref uint randState)
        {
            float3 rd = camera.lensRadius * RandUtils.RandomInUnitDisk(ref randState);
            float3 offset = camera.u * rd.X + camera.v * rd.Y;

            Ray ray;
            ray.origin = camera.origin + offset;
            ray.direction = camera.lowerLeftCorner + u * camera.horizontal + v * camera.vertical - camera.origin - offset;
            return ray;
        }
    }
}
