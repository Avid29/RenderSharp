// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Scene.Rays;
using RenderSharp.Utilities.Tiles;

namespace RenderSharp.RayTracing.Shaders.Shading.Stock.SkyShaders;

/// <summary>
/// A solid color sky shader.
/// </summary>
[AutoConstructor]
[EmbeddedBytecode(DispatchAxis.XY)]
public partial struct SolidSkyShader : IComputeShader
{
    private readonly Tile tile;
    private readonly float4 color;
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

        // If the sky was not hit do not execute
        if (rayCastBuffer[fIndex].triId != -1)
            return;

        // Add to the color buffer
        renderBuffer[imageIndex] += color * attenuationBuffer[index2D];

        // Clear the ray in the ray buffer
        rayBuffer[fIndex].origin = 0;
        rayBuffer[fIndex].direction = 0;
        rayCastBuffer[fIndex].matId = -2;
        rayCastBuffer[fIndex].triId = -2;
    }
}
