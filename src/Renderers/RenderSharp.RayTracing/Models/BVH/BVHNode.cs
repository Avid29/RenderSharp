// Adam Dernis 2023

using RenderSharp.RayTracing.RayCasts;

namespace RenderSharp.RayTracing.Models.BVH;

/// <summary>
/// A node in a BVH tree.
/// </summary>
public struct BVHNode
{
    /// <summary>
    /// The axis-aligned bounding box associated to the node.
    /// </summary>
    public AABB box;

    /// <summary>
    /// The index of the left child in the tree.
    /// </summary>
    /// <remarks>
    /// -1 if leaf node.
    /// </remarks>
    public int leftIndex;
    
    /// <summary>
    /// The index of the right child in the tree.
    /// </summary>
    /// <remarks>
    /// -1 if leaf node.
    /// </remarks>
    public int rightIndex;

    /// <summary>
    /// The index of the contained triangle, if leaf node.
    /// </summary>
    /// <remarks>
    /// -1 if not a leaf node.
    /// </remarks>
    public int geoIndex;

    /// <summary>
    /// Creates a new branch <see cref="BVHNode"/>.
    /// </summary>
    /// <param name="box">The <see cref="BVHNode"/>'s AABB.</param>
    /// <param name="leftIndex">The index of the left child node.</param>
    /// <param name="rightIndex">The index of the right child node.</param>
    /// <returns>A new <see cref="BVHNode"/>.</returns>
    public static BVHNode Create(AABB box, int leftIndex, int rightIndex)
        => Create(box, leftIndex, rightIndex, -1);
    
    /// <summary>
    /// Creates a new leaf <see cref="BVHNode"/>.
    /// </summary>
    /// <param name="box">The <see cref="BVHNode"/>'s AABB.</param>
    /// <param name="geoIndex">The index of the wrapped triangle in the geometry buffer.</param>
    /// <returns>A new <see cref="BVHNode"/>.</returns>
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

    /// <summary>
    /// Gets whether or not a ray intersects a <see cref="BVHNode"/>.
    /// </summary>
    /// <param name="node">The <see cref="BVHNode"/>.</param>
    /// <param name="ray">The <see cref="Ray"/>.</param>
    /// <returns>True if intersecting.</returns>
    public static bool IsHit(BVHNode node, Ray ray)
        => IsHit(node, ray, float.MaxValue);
    
    /// <summary>
    /// Gets whether or not a ray intersects a <see cref="BVHNode"/> before clipping.
    /// </summary>
    /// <param name="node">The <see cref="BVHNode"/>.</param>
    /// <param name="ray">The <see cref="Ray"/>.</param>
    /// <param name="maxClip">The max intersection distance.</param>
    /// <returns>True if intersecting.</returns>
    public static bool IsHit(BVHNode node, Ray ray, float maxClip)
        => AABB.IsHit(node.box, ray, maxClip, float.Epsilon);
}
