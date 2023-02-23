// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Models.Geometry;
using RenderSharp.RayTracing.Models.Rays;

namespace RenderSharp.RayTracing.Shaders.Rendering;

/// <summary>
/// An <see cref="IComputeShader"/> that creates detects geometry collisions.
/// </summary>
[AutoConstructor]
[EmbeddedBytecode(DispatchAxis.XYZ)]
public partial struct ShadowIntersectionShader : IComputeShader
{
    private readonly ReadOnlyBuffer<Vertex> vertexBuffer;
    private readonly ReadOnlyBuffer<Triangle> geometryBuffer;
    private readonly ReadWriteBuffer<Ray> shadowCastBuffer;

    /// <inheritdoc/>
    public void Execute()
    {
        // Get the index of resources managed by the current thread
        // in 2D and 3D textures as well as flat buffers
        int2 index2D = ThreadIds.XY;
        int3 index3D = ThreadIds.XYZ;
        int fPxlIndex = (index2D.Y * DispatchSize.X) + index2D.X;
        int lightIndex = ThreadIds.Z;
        int fLightIndex = (index3D.Z * DispatchSize.X * DispatchSize.Y) + (index3D.Y * DispatchSize.X) + index3D.X;

        Ray ray = shadowCastBuffer[fLightIndex];
        if (Hlsl.Length(ray.direction) == 0)
            return;

        // Check for collision with every triangle in the geometry buffer
        // TODO: Use bounding volume hierarchy (BVH tree) to decrease collision search time
        for (int i = 0; i < geometryBuffer.Length; i++)
        {
            var tri = geometryBuffer[i];
            VertexTriangle vTri;
            vTri.triangle = tri;
            vTri.a = vertexBuffer[tri.a];
            vTri.b = vertexBuffer[tri.b];
            vTri.c = vertexBuffer[tri.c];
            if (VertexTriangle.IsHit(vTri, ray, out var cast))
            {
                shadowCastBuffer[fLightIndex].direction = float3.Zero;
                return;
            }
        }
    }
}
