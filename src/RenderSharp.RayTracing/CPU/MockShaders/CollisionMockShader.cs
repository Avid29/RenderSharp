using ComputeSharp;
using Microsoft.Toolkit.HighPerformance;
using RenderSharp.RayTracing.Scenes;
using RenderSharp.RayTracing.Scenes.BVH;
using RenderSharp.RayTracing.Scenes.Geometry;
using RenderSharp.RayTracing.Scenes.Rays;
using System;

namespace RenderSharp.RayTracing.CPU.MockShaders
{
    [AutoConstructor]
    public ref partial struct CollisionMockShader
    {
        private readonly Scene _scene;
        private readonly ReadOnlySpan<Triangle> _geometryBuffer;
        private readonly ReadOnlySpan<BVHNode> _bvhHeap;

        private readonly int[,,] _bvhStack;
        private readonly Span<Ray> _rayBuffer;
        private readonly Span<RayCast> _rayCastBuffer;
        private readonly Span2D<int> _materialBuffer;

        public void Execute(int width, int height)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int bPos = y * width + x;

                    Ray ray = _rayBuffer[bPos];

                    float nearest = float.MaxValue;
                    if (_materialBuffer[x, y] >= 0) _materialBuffer[x, y] = -1;

                    int stackIndex = 0;
                    _bvhStack[x, y, stackIndex] = 0;

                    do
                    {
                        int nodeIndex = _bvhStack[x, y, stackIndex--];
                        BVHNode node = _bvhHeap[nodeIndex];

                        if (BVHNode.IsHit(node, ray, nearest))
                        {
                            if (node.geoI != -1)
                            {
                                Triangle triangle = _geometryBuffer[node.geoI];
                                if (Triangle.IsHit(triangle, ray, out RayCast newCast))
                                {
                                    nearest = newCast.coefficient;
                                    _materialBuffer[x, y] = 0;
                                    _rayCastBuffer[bPos] = newCast;
                                }
                            }
                            else
                            {
                                _bvhStack[x, y, ++stackIndex] = node.leftI;
                                _bvhStack[x, y, ++stackIndex] = node.rightI;
                            }
                        }
                    } while (stackIndex != -1);
                }
            }
        }
    }
}
