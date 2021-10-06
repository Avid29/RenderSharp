using ComputeSharp;
using System.Numerics;

namespace RenderSharp.Common.Scenes
{
    public class Camera
    {
        public Camera(Vector3 origin, Vector3 look, float fov, float aperture)
        {
            Origin = origin;
            Look = look;
            FocalLength = Vector3.Distance(origin, look);
            FOV = fov;
            Aperture = aperture;
        }

        public Camera(Vector3 origin, Vector3 look, float focalLength, float fov, float aperture)
        {
            Origin = origin;
            Look = look;
            FocalLength = focalLength;
            FOV = fov;
            Aperture = aperture;
        }

        public Vector3 Origin { get; }

        public Vector3 Look { get; }

        public float FOV { get; }

        public float FocalLength { get; }

        public float Aperture { get; }
    }
}
