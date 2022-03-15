using System.Numerics;

namespace RenderSharp.RayTracing.Scenes.Cameras
{
    public struct Camera
    {
        public Vector3 origin;
        public Vector3 look;
        public float focalLength;
        public float fov;
        public float aperture;

        public static Camera Create(Vector3 origin, Vector3 look, float focalLength, float fov, float aperture)
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
