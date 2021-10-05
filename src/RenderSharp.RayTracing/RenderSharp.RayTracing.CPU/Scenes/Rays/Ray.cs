using System.Numerics;

namespace RenderSharp.RayTracing.CPU.Scenes.Rays
{
    public struct Ray
    {
        public Ray(Vector3 origin, Vector3 direction)
        {
            Origin = origin;

            Direction = direction;
        }

        public Vector3 Origin { get; }

        public Vector3 Direction { get; }

        public Vector3 PointAt(float t)
        {
            return Origin + t * Direction;
        }
    }
}
