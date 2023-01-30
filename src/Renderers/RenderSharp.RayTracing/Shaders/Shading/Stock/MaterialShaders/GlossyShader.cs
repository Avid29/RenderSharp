// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Scene.Rays;
using RenderSharp.Utilities.Tiles;

namespace RenderSharp.RayTracing.Shaders.Shading.Stock.MaterialShaders;

/// <summary>
/// A solid color glossy material shader.
/// </summary>
[AutoConstructor]
[EmbeddedBytecode(DispatchAxis.XY)]
public partial struct GlossyShader : IComputeShader
{
    private readonly Tile tile;
    private readonly int matId;
    private readonly GlossyMaterial material;

    private readonly ReadWriteBuffer<Ray> rayBuffer;
    private readonly ReadWriteBuffer<RayCast> rayCastBuffer;
    private readonly IReadWriteNormalizedTexture2D<float4> attenuationBuffer;
    private readonly IReadWriteNormalizedTexture2D<float4> renderBuffer;

    /// <inheritdoc/>
    public void Execute()
    {
        // Get the index of resources managed by the current thread
        // in both 2D textures and flat buffers
        int2 index2D = ThreadIds.XY;
        int fIndex = (index2D.Y * DispatchSize.X) + index2D.X;
        int2 imageIndex = index2D + tile.offset;

        var cast = rayCastBuffer[fIndex];
        var ray = rayBuffer[fIndex];

        // If this material was not hit do not execute
        if (cast.matId != matId)
            return;

        float3 target = Hlsl.Reflect(Hlsl.Normalize(ray.direction), cast.normal);
        Ray scatter = Ray.Create(cast.position, target - cast.position);

        rayBuffer[fIndex] = scatter;
        attenuationBuffer[index2D] *= material.albedo;
    }
}
