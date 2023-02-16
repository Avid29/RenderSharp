// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Scene.Geometry;
using RenderSharp.RayTracing.Scene.Rays;
using RenderSharp.Utilities.Tiles;

namespace RenderSharp.RayTracing.Shaders.Rendering;

/// <summary>
/// An <see cref="IComputeShader"/> that creates detects geometry collisions.
/// </summary>
[AutoConstructor]
[EmbeddedBytecode(DispatchAxis.XYZ)]
public partial struct ShadowIntersectionShader : IComputeShader
{
    private readonly Tile tile;
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
            if (Triangle.IsHit(tri, ray, out var cast))
            {
                shadowCastBuffer[fLightIndex].direction = float3.Zero;
                return;
            }
        }
    }
}
