using RenderSharp.RayTracing.CPU.Geometry;
using RenderSharp.RayTracing.CPU.Materials;
using RenderSharp.RayTracing.CPU.Rays;
using RenderSharp.RayTracing.CPU.Skys;

namespace RenderSharp.RayTracing.CPU.Components
{
    public struct World
    {
        public World(Sky sky, IGeometry[] geometries)
        {
            Sky = sky;
            Geometries = geometries;
        }

        public Sky Sky { get; }

        public IGeometry[] Geometries { get; }

        public bool GetHit(Ray ray, out RayCast cast, out IMaterial material)
        {
            cast = new RayCast();
            material = null;

            bool hit = false;
            float closest = float.MaxValue;

            RayCast cacheCast;
            for (int i = 0; i < Geometries.Length; i++)
            {
                IGeometry geometry = Geometries[i];
                if (geometry.IsHit(ray, closest, out cacheCast))
                {
                    hit = true;
                    closest = cacheCast.Time;
                    cast = cacheCast;
                    material = geometry.Material;
                }
            }

            return hit;
        }
    }
}
