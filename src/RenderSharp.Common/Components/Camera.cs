using ComputeSharp;
using System.Numerics;

namespace RenderSharp.Common.Components
{
    public class Camera
    {
        public Camera(Float3 origin, Float3 look, float fov, float aperture)
        {
            Origin = origin;
            Look = look;
            FocalLength = Vector3.Distance(origin, look);
            FOV = fov;
            Aperture = aperture;
        }

        public Camera(Float3 origin, Float3 look, float focalLength, float fov, float aperture)
        {
            Origin = origin;
            Look = look;
            FocalLength = focalLength;
            FOV = fov;
            Aperture = aperture;
        }

        public Float3 Origin { get; }

        public Float3 Look { get; }

        public float FOV { get; }

        public float FocalLength { get; }

        public float Aperture { get; }
    }
}
