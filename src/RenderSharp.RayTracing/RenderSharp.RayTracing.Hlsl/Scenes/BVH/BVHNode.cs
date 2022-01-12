using RenderSharp.RayTracing.HLSL.Scenes.Rays;

namespace RenderSharp.RayTracing.HLSL.Scenes.BVH
{
    public struct BVHNode
    {
        public AABB boundingBox;
        public int leftI;
        public int rightI;
        public int geoI;

        public static bool IsHit(BVHNode node, RayCast ray, float maxClip)
        {
            return AABB.IsHit(node.boundingBox, ray, maxClip, 0.0001f);
        }
    }
}
