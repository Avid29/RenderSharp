using ComputeSharp;
using RenderSharp.RayTracing.Scenes;
using RenderSharp.RayTracing.Scenes.BVH;
using RenderSharp.RayTracing.Scenes.Geometry;
using RenderSharp.RayTracing.Scenes.Rays;

namespace RenderSharp.RayTracing.GPU.Shaders
{
    [AutoConstructor]
    [EmbeddedBytecode(DispatchAxis.XY)]
    public partial struct CollisionShader : IComputeShader
    {
        private readonly Scene scene;
        private readonly ReadOnlyBuffer<Triangle> geometryBuffer;
        private readonly ReadOnlyBuffer<BVHNode> bvhHeap;

        private readonly ReadWriteTexture3D<int> bvhStack;
        private readonly ReadWriteBuffer<Ray> rayBuffer;
        private readonly ReadWriteBuffer<RayCast> rayCastBuffer;
        private readonly ReadWriteTexture2D<int> materialBuffer;

        public void Execute()
        {
            Int2 pos = ThreadIds.XY;
            Int2 dis = DispatchSize.XY;
            int bPos = pos.Y * dis.X + pos.X;

            Ray ray = rayBuffer[bPos];

            // Cache nearest hit info
            float nearest = float.MaxValue;
            if (materialBuffer[pos] >= 0) materialBuffer[pos] = -1; // -1 Means the sky was hit

            // Add the root BVH node to the stack
            int stackIndex = 0;
            bvhStack[pos.X, pos.Y, stackIndex] = 0;

            // Traverse the BVH Tree
            do
            {
                // Pop stack
                int nodeIndex = bvhStack[pos.X, pos.Y, stackIndex--];
                BVHNode node = bvhHeap[nodeIndex];

                if (BVHNode.IsHit(node, ray, nearest))
                {
                    if (node.geoI != -1)
                    {
                        Triangle triangle = geometryBuffer[node.geoI];
                        if (Triangle.IsHit(triangle, ray, out RayCast newCast))
                        {
                            nearest = newCast.coefficient;
                            materialBuffer[pos] = triangle.matId;
                            rayCastBuffer[bPos] = newCast;
                        }
                    }
                    else
                    {
                        // Push stack
                        bvhStack[pos.X, pos.Y, ++stackIndex] = node.leftI;
                        bvhStack[pos.X, pos.Y, ++stackIndex] = node.rightI;
                    }
                }

            } while (stackIndex != -1);
        }
    }
}
