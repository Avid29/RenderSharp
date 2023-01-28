﻿// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Scene.Geometry;
using RenderSharp.RayTracing.Scene.Rays;

namespace RenderSharp.RayTracing.Shaders.Rendering;

/// <summary>
/// An <see cref="IComputeShader"/> that creates detects geometry collisions.
/// </summary>
[AutoConstructor]
[EmbeddedBytecode(DispatchAxis.XY)]
public readonly partial struct GeometryCollisionShader : IComputeShader
{
    private readonly ReadOnlyBuffer<Triangle> geometryBuffer;
    private readonly ReadWriteBuffer<Ray> rayBuffer;
    private readonly ReadWriteBuffer<RayCast> rayCastBuffer;

    public void Execute()
    {
        // Get the index of resources managed by the current thread
        // in both 2D textures and flat buffers
        int2 index2D = ThreadIds.XY;
        int fIndex = (index2D.Y * DispatchSize.X) + index2D.X;

        Ray ray = rayBuffer[fIndex];
        var rayCast = RayCast.Create(0, 0, 0);

        // Track the nearest scene collision
        float distance = float.MaxValue;

        // Check for collision with every triangle in the geometry buffer
        // TODO: Use bounding volume hierarchy (BVH tree) to decrease collision search time
        for (int i = 0; i < geometryBuffer.Length; i++)
        {
            // Check for a closer collision, and log its ray cast when any exist.
            var tri = geometryBuffer[i];
            if (Triangle.IsHit(tri, ray, distance, out var cast))
            {
                distance = cast.distance;
                cast.triId = i;
                rayCast = cast;
            }
        }

        // Store the ray cast in the ray cast buffer
        rayCastBuffer[fIndex] = rayCast;
    }
}
