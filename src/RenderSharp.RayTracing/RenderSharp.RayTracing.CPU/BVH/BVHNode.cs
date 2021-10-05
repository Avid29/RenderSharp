using RenderSharp.RayTracing.CPU.Geometry;
using RenderSharp.RayTracing.CPU.Rays;
using System;
using System.Numerics;

namespace RenderSharp.RayTracing.CPU.BVH
{
    public class BVHNode
    {
        public BVHNode(Span<IGeometry> geometries, int index, int depth)
        {
            // Rotate the split axis as we go down the BVH tree.
            int axis = depth;

            if (geometries.Length == 1)
            {
                GeometryIndex = index;
                BoundingBox = geometries[0].GetBoundingBox();
            } else
            {
                geometries.Sort((a, b) => a.GetBoundingBox().Maximum.At(axis).CompareTo(b.GetBoundingBox().Maximum.At(axis)));

                int mid = geometries.Length / 2;
                Left = new BVHNode(geometries.Slice(0, mid), index, depth+1);
                Right = new BVHNode(geometries.Slice(mid, geometries.Length - mid), index + mid, depth+1);

                BoundingBox = AABB.GetSurroundingBox(Left.BoundingBox, Right.BoundingBox);
                GeometryIndex = -1;
            }
        }

        public AABB BoundingBox { get; }

        public BVHNode Left { get; }

        public BVHNode Right { get; }

        public int GeometryIndex { get; }

        public bool IsHit(Ray ray, float maxClip)
        {
            return BoundingBox.IsHit(ray, maxClip, 0.0001f);
        }
    }
}
