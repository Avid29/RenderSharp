using ComputeSharp;

namespace RenderSharp.RayTracing.HLSL.Scenes.Cameras
{
    public struct Camera
    {
        public Float3 origin;
        public Float3 look;
        public float focalLength;
        public float fov;
        public float aperture;

        public static Camera CreateCamera(Float3 origin, Float3 look, float focalLength, float fov, float aperture)
        {
            Camera camera;
            camera.origin = origin;
            camera.look = look;
            camera.fov = fov;
            camera.focalLength = focalLength;
            camera.aperture = aperture;
            return camera;
        }
    }
}
