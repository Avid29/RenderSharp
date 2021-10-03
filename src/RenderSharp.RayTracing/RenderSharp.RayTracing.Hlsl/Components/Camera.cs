using ComputeSharp;

namespace RenderSharp.RayTracing.HLSL.Components
{
    public struct Camera
    {
        public Float3 origin;
        public Float3 look;
        public float focalLength;
        public float fov;

        public static Camera CreateCamera(Float3 origin, Float3 look, float focalLength, float fov)
        {
            Camera camera;
            camera.origin = origin;
            camera.look = look;
            camera.focalLength = focalLength;
            camera.fov = fov;
            return camera;
        }
    }
}
