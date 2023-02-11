// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Scene.BVH;
using RenderSharp.RayTracing.Scene.Geometry;
using RenderSharp.RayTracing.Scene.Rays;
using RenderSharp.Utilities.Tiles;

namespace RenderSharp.RayTracing.Shaders.Rendering;

/// <summary>
/// An <see cref="IComputeShader"/> that creates detects geometry collisions.
/// </summary>
[AutoConstructor]
[EmbeddedBytecode(DispatchAxis.XY)]
public readonly partial struct GeometryCollisionBVHTreeShader : IComputeShader
{
    private readonly Tile tile;
    private readonly ReadWriteTexture3D<int> bvhStackBuffer;
    private readonly ReadOnlyBuffer<BVHNode> bvhTreeBuffer;
    private readonly ReadOnlyBuffer<Triangle> geometryBuffer;
    private readonly ReadWriteBuffer<Ray> rayBuffer;
    private readonly ReadWriteBuffer<RayCast> rayCastBuffer;

    /// <inheritdoc/>
    public void Execute()
    {
        // Get the index of resources managed by the current thread
        // in both 2D textures and flat buffers
        int2 index2D = ThreadIds.XY;
        int fIndex = (index2D.Y * DispatchSize.X) + index2D.X;

        Ray ray = rayBuffer[fIndex];
        if (Hlsl.Length(ray.direction) == 0)
            return;

        var rayCast = RayCast.Create(0, 0, 0);

        // Track the nearest scene collision
        float distance = float.MaxValue;
        
        // Add the root BVH node to the stack
        int stackIndex = 0;
        bvhStackBuffer[index2D.X, index2D.Y, stackIndex] = 0;
        
        // Traverse the BVH Tree
        do
        {
            // Pop stack
            int nodeIndex = bvhStackBuffer[index2D.X, index2D.Y, stackIndex--];
            BVHNode node = bvhTreeBuffer[nodeIndex];
            
            // Check for a closer collision, and log its ray cast when any exist.
            if (BVHNode.IsHit(node, ray, distance))
            {
                if (node.geoIndex != -1)
                {
                    Triangle triangle = geometryBuffer[node.geoIndex];
                    if (Triangle.IsHit(triangle, ray, out RayCast cast))
                    {
                        distance = cast.distance;
                        cast.triId = node.geoIndex;
                        cast.matId = triangle.matId;
                        rayCast = cast;
                    }
                }
                else
                {
                    // Push stack
                    bvhStackBuffer[index2D.X, index2D.Y, ++stackIndex] = node.leftIndex;
                    bvhStackBuffer[index2D.X, index2D.Y, ++stackIndex] = node.rightIndex;
                }
            }
        } while (stackIndex != -1);

        // Store the ray cast in the ray cast buffer
        rayCastBuffer[fIndex] = rayCast;
    }
}
