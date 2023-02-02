// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Scene.BVH;
using RenderSharp.RayTracing.Scene.Geometry;
using System.Runtime.InteropServices;

namespace RenderSharp.RayTracing.Setup;

public class BVHBuilder
{
    private readonly List<Triangle> _triangles;
    private readonly BVHNode[] _bvhHeap;
    private int _maxDepth;
    private int _pos;

    public BVHBuilder(GraphicsDevice device, List<Triangle> geometries)
    {
        Device = device;
        _triangles = geometries;
        _bvhHeap = new BVHNode[(geometries.Count * 2) - 1];
    }

    /// <summary>
    /// Gets the device for buffer allocation.
    /// </summary>
    public GraphicsDevice Device { get; }

    public void BuildBVHTree()
    {
        _maxDepth = 0;
        _pos = _bvhHeap.Length - 1;
        var geometries = CollectionsMarshal.AsSpan(_triangles);
        BuildBVH(geometries, 0, 0);
    }

    private int BuildBVH(Span<Triangle> geometries, int index, int depth)
    {
        int axis = depth;

        _maxDepth = int.Max(_maxDepth, depth);

        BVHNode node;

        if (geometries.Length == 1)
        {
            AABB box = Triangle.GetAABB(geometries[0]);
            node = BVHNode.Create(box, index);
        }
        else
        {
            // TODO: Split on a better basis
            geometries.Sort((a, b) => Triangle.GetAABB(a).highCorner[axis].CompareTo(Triangle.GetAABB(b).highCorner));

            int mid = geometries.Length / 2;
            int rightI = BuildBVH(geometries[mid..], index + mid, depth + 1);
            int leftI = BuildBVH(geometries[..mid], index, depth + 1);

            AABB rightBB = _bvhHeap[rightI].box;
            AABB leftBB = _bvhHeap[leftI].box;

            AABB box = AABB.GetSurroundingBox(leftBB, rightBB);
            node = BVHNode.Create(box, leftI, rightI);
        }

        _bvhHeap[_pos] = node;
        return _pos--;
    }
}
