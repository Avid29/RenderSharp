namespace RenderSharp.RayTracing.Scenes.Cameras
{
    public struct Camera
    {
        public float3 origin;
        public float3 look;
        public float focalLength;
        public float fov;
        public float aperture;

        public static Camera CreateCamera(float3 origin, float3 look, float focalLength, float fov, float aperture)
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
