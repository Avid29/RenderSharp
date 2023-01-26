// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Models.Geometry;
using RenderSharp.RayTracing.Models.Rays;

namespace RenderSharp.RayTracing.Shaders;

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

        float distance = float.MaxValue;

        // TODO: Use bounding volume hierarchy (BVH tree) to decrease collision search time
        for (int i = 0; i < geometryBuffer.Length; i++)
        {
            var tri = geometryBuffer[i];
            if (Triangle.IsHit(tri, ray, distance, out var cast))
            {
                distance = cast.distance;
                cast.objId = i;
                rayCast = cast;
            }
        }

        rayCastBuffer[fIndex] = rayCast;
    }
}
