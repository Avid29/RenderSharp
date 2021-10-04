using RenderSharp.RayTracing.CPU.Materials;
using RenderSharp.RayTracing.CPU.Rays;

namespace RenderSharp.RayTracing.CPU.Geometry
{
    public interface IGeometry
    {
        public IMaterial Material { get; }

        public bool IsHit(Ray ray, out RayCast cast);

        public bool IsHit(Ray ray, float maxClip, out RayCast cast);
    }
}
