using RenderSharp.RayTracing.CPU.Scenes.BVH;
using RenderSharp.RayTracing.CPU.Scenes.Materials;
using RenderSharp.RayTracing.CPU.Scenes.Rays;

namespace RenderSharp.RayTracing.CPU.Scenes.Geometry
{
    public interface IGeometry
    {
        public IMaterial Material { get; }

        public bool IsHit(Ray ray, out RayCast cast);

        public bool IsHit(Ray ray, float maxClip, out RayCast cast);

        public AABB GetBoundingBox();
    }
}
