using RenderSharp.RayTracing.Scenes.Rays;

namespace RenderSharp.RayTracing.Scenes.BVH
{
    public struct BVHNode
    {
        public AABB boundingBox;
        public int leftI;
        public int rightI;
        public int geoI;

        public static BVHNode Create(AABB aabb, int geoI) => Create(aabb, -1, -1, geoI);

        public static BVHNode Create(AABB aabb, int leftI, int rightI) => Create(aabb, leftI, rightI, -1);

        public static BVHNode Create(AABB aabb, int leftI, int rightI, int geoI)
        {
            BVHNode node;
            node.boundingBox = aabb;
            node.leftI = leftI;
            node.rightI = rightI;
            node.geoI = geoI;
            return node;
        }

        public static bool IsHit(BVHNode node, Ray ray, float maxClip)
        {
            return AABB.IsHit(node.boundingBox, ray, maxClip, 0.0001f);
        }
    }
}
