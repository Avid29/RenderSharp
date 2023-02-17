// Adam Dernis 2023

using RenderSharp.RayTracing.Models.Rays;

namespace RenderSharp.RayTracing.Models.BVH;

public struct BVHNode
{
    public AABB box;
    public int leftIndex;
    public int rightIndex;
    public int geoIndex;

    public static BVHNode Create(AABB box, int leftIndex, int rightIndex)
        => Create(box, leftIndex, rightIndex, -1);

    public static BVHNode Create(AABB box, int geoIndex)
        => Create(box, -1, -1, geoIndex);

    private static BVHNode Create(AABB box, int leftIndex, int rightIndex, int geoIndex)
    {
        BVHNode node;
        node.box = box;
        node.leftIndex = leftIndex;
        node.rightIndex = rightIndex;
        node.geoIndex = geoIndex;
        return node;
    }

    public static bool IsHit(BVHNode node, Ray ray)
        => IsHit(node, ray, float.MaxValue);

    public static bool IsHit(BVHNode node, Ray ray, float maxClip)
        => AABB.IsHit(node.box, ray, maxClip, float.Epsilon);
}
