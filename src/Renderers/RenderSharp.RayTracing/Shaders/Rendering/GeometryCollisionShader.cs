// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Models.Geometry;
using RenderSharp.RayTracing.Models.Rays;

namespace RenderSharp.RayTracing.Shaders.Rendering;

/// <summary>
/// An <see cref="IComputeShader"/> that creates detects geometry collisions.
/// </summary>
[AutoConstructor]
[EmbeddedBytecode(DispatchAxis.XY)]
public readonly partial struct GeometryCollisionShader : IComputeShader
{
    private readonly ReadOnlyBuffer<Vertex> vertexBuffer;
    private readonly ReadOnlyBuffer<Triangle> geometryBuffer;
    private readonly ReadWriteBuffer<Ray> rayBuffer;
    private readonly ReadWriteBuffer<GeometryCollision> rayCastBuffer;

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

        var rayCast = GeometryCollision.Create(0, 0, 0, 0, 0);

        // Track the nearest scene collision
        float distance = float.MaxValue;

        // Check for collision with every triangle in the geometry buffer
        for (int i = 0; i < geometryBuffer.Length; i++)
        {
            var tri = geometryBuffer[i];
            VertexTriangle vTri;
            vTri.triangle = tri;
            vTri.a = vertexBuffer[tri.a];
            vTri.b = vertexBuffer[tri.b];
            vTri.c = vertexBuffer[tri.c];

            if (VertexTriangle.IsHit(vTri, ray, distance, out var cast))
            {
                distance = cast.distance;
                cast.geoId = i;
                cast.matId = tri.matId;
                cast.objId = tri.objId;
                rayCast = cast;
            }
        }

        // Store the ray cast in the ray cast buffer
        rayCastBuffer[fIndex] = rayCast;
    }
}
