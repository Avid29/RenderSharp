using ComputeSharp;

namespace RenderSharp.RayTracing.HLSL.Components
{
    public struct Camera
    {
        public Float3 origin;
        public Float3 look;
        public float focalLength;
        public float fov;
        public float aperature;

        public static Camera CreateCamera(Float3 origin, Float3 look, float focalLength, float fov, float aperature)
        {
            Camera camera;
            camera.origin = origin;
            camera.look = look;
            camera.fov = fov;
            camera.focalLength = focalLength;
            camera.aperature = aperature;
            return camera;
        }
    }
}
