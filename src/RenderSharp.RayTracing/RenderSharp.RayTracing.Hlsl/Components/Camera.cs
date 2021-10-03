using ComputeSharp;

namespace RenderSharp.RayTracing.HLSL.Components
{
    public struct Camera
    {
        public Float3 origin;
        public float focalLength;
        public float fov;

        public static Camera CreateCamera(Float3 origin, float focalLength, float fov)
        {
            Camera camera;
            camera.origin = origin;
            camera.focalLength = focalLength;
            camera.fov = fov;
            return camera;
        }
    }
}
