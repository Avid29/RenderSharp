// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Models.Lighting;
using RenderSharp.RayTracing.Models.Rays;

namespace RenderSharp.RayTracing.Shaders.Rendering;

[AutoConstructor]
[EmbeddedBytecode(DispatchAxis.XYZ)]
public partial struct ShadowCastShader : IComputeShader
{
    private readonly ReadOnlyBuffer<Light> lightsBuffer;
    private readonly ReadWriteBuffer<Ray> shadowCastBuffer;
    private readonly ReadWriteBuffer<GeometryCollision> rayCastBuffer;

    public void Execute()
    {
        // Get the index of resources managed by the current thread
        // in 2D and 3D textures as well as flat buffers
        int2 index2D = ThreadIds.XY;
        int3 index3D = ThreadIds.XYZ;
        int fPxlIndex = (index2D.Y * DispatchSize.X) + index2D.X;
        int lightIndex = ThreadIds.Z;
        int fLightIndex = (index3D.Z * DispatchSize.X * DispatchSize.Y) + (index3D.Y * DispatchSize.X) + index3D.X;

        var origin = rayCastBuffer[fPxlIndex].position;
        var direction = lightsBuffer[lightIndex].position - origin;

        shadowCastBuffer[fLightIndex] = Ray.Create(origin, Hlsl.Normalize(direction));
    }
}
