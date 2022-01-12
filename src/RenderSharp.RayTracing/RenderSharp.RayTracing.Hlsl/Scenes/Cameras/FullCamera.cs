using ComputeSharp;
using RenderSharp.RayTracing.HLSL.Scenes.Rays;
using RenderSharp.RayTracing.HLSL.Utils;
using System;

namespace RenderSharp.RayTracing.HLSL.Scenes.Cameras
{
    public struct FullCamera
    {
        public Float3 origin;
        public Float3 horizontal;
        public Float3 vertical;
        public Float3 lowerLeftCorner;
        public Float3 u, v, w;
        public float lensRadius;

        public static FullCamera Create(Camera specs, float aspectRatio)
        {
            float theta = FloatUtils.DegreesToRadians(specs.fov);
            float h = MathF.Tan(theta/2);
            float height = 2 * h;
            float width = aspectRatio * height;

            Float3 vup = Float3.UnitY;

            FullCamera camera;
            camera.origin = specs.origin;
            camera.w = Hlsl.Normalize(specs.origin - specs.look);
            camera.u = Hlsl.Normalize(Hlsl.Cross(vup, camera.w));
            camera.v = Hlsl.Cross(camera.w, camera.u);
            camera.horizontal = width * camera.u;
            camera.vertical = height * camera.v;
            Float3 depth = camera.w * specs.focalLength;

            camera.lowerLeftCorner = camera.origin - camera.horizontal / 2 - camera.vertical / 2 - depth;

            camera.lensRadius = specs.aperture / 2;
            return camera;
        }

        public static RayCast CreateRay(FullCamera camera, float u, float v, ref uint randState)
        {
            Float3 rd = camera.lensRadius * RandUtils.RandomInUnitDisk(ref randState);
            Float3 offset = camera.u * rd.X + camera.v * rd.Y;

            RayCast ray;
            ray.origin = camera.origin + offset;
            ray.normal = camera.lowerLeftCorner + u * camera.horizontal + v * camera.vertical - camera.origin - offset;
            ray.coefficient = -1;
            return ray;
        }
    }
}
