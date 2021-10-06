using RenderSharp.RayTracing.HLSL.Rays;

namespace RenderSharp.RayTracing.HLSL.BVH
{
    public struct BVHNode
    {
        public AABB boundingBox;
        public int leftI;
        public int rightI;
        public int geoI;

        public static bool IsHit(BVHNode node, Ray ray, float maxClip)
        {
            return AABB.IsHit(node.boundingBox, ray, maxClip, 0.0001f);
        }
    }
}
