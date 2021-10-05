using System.Numerics;

namespace RenderSharp.RayTracing.CPU.Scenes.Rays
{
    public struct RayCast
    {
        public RayCast(Vector3 origin, Vector3 normal, float time)
        {
            Origin = origin;
            Normal = normal;
            Time = time;
        }

        public Vector3 Origin { get; }

        public Vector3 Normal { get; }

        public float Time { get; }
    }
}
