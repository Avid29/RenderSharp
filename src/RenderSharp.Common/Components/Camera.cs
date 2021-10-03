using ComputeSharp;

namespace RenderSharp.Common.Components
{
    public class Camera
    {
        public Camera(Float3 origin, Float3 look, float fov, float focalLength)
        {
            Origin = origin;
            Look = look;
            FocalLength = focalLength;
            FOV = fov;
        }

        public Float3 Origin { get; }

        public Float3 Look { get; }

        public float FOV { get; }

        public float FocalLength { get; }
    }
}
