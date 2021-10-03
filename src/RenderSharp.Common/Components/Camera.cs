using ComputeSharp;

namespace RenderSharp.Common.Components
{
    public class Camera
    {
        public Camera(Float3 origin, float fov, float focalLength)
        {
            Origin = origin;
            FocalLength = focalLength;
            FOV = fov;
        }

        public Float3 Origin { get; }

        public float FOV { get; }

        public float FocalLength { get; }
    }
}
