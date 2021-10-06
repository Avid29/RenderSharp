using RenderSharp.RayTracing.CPU.BVH;
using RenderSharp.RayTracing.CPU.Geometry;
using RenderSharp.RayTracing.CPU.Materials;
using RenderSharp.RayTracing.CPU.Rays;
using RenderSharp.RayTracing.CPU.Skys;
using System.Collections.Generic;

namespace RenderSharp.RayTracing.CPU.Components
{
    public struct World
    {
        public World(Sky sky, IGeometry[] geometries)
        {
            Sky = sky;
            Geometries = geometries;
            BVHTree = new BVHNode(Geometries, 0, 0);
        }

        public Sky Sky { get; }

        public IGeometry[] Geometries { get; }

        public BVHNode BVHTree { get; }

        public bool GetHit(Ray ray, out RayCast cast, out IMaterial material)
        {
            return GetHitBVH(ray, out cast, out material);
        }

        private bool GetHitBVH(Ray ray, out RayCast cast, out IMaterial material)
        {
            cast = new RayCast();
            material = null;

            bool hit = false;
            float closest = float.MaxValue;

            RayCast cacheCast;
            Stack<BVHNode> stack = new Stack<BVHNode>();
            stack.Push(BVHTree);

            BVHNode node;
            while (stack.Count > 0)
            {
                node = stack.Pop();
                if (node.IsHit(ray, closest))
                {
                    if (node.GeometryIndex != -1)
                    {
                        // Leaf node
                        IGeometry geometry = Geometries[node.GeometryIndex];
                        if (geometry.IsHit(ray, out cacheCast))
                        {
                            hit = true;
                            closest = cacheCast.Time;
                            cast = cacheCast;
                            material = geometry.Material;
                        }
                    }
                    else
                    {
                        stack.Push(node.Left);
                        stack.Push(node.Right);
                    }
                }
            }

            return hit;
        }

        private bool GetHitNoBVH(Ray ray, out RayCast cast, out IMaterial material)
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
