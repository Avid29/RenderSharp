// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Scene.Rays;

namespace RenderSharp.RayTracing.Shaders.Debugging;

/// <summary>
/// An <see cref="IComputeShader"/> that dumps a property from the <see cref="Ray"/> buffer as a color.
/// </summary>
[AutoConstructor]
[EmbeddedBytecode(DispatchAxis.XY)]
public readonly partial struct RayBufferDumpShader : IComputeShader
{
    private readonly ReadWriteBuffer<Ray> rayBuffer;
    private readonly IReadWriteNormalizedTexture2D<float4> renderBuffer;

    /// <remarks>
    /// TODO: Use enum
    /// Waiting on https://github.com/Sergio0694/ComputeSharp/issues/248
    /// </remarks>
    private readonly int dumpType;

    /// <inheritdoc/>
    public void Execute()
    {
        // Get the index of resources managed by the current thread
        // in both 2D textures and flat buffers
        int2 index2D = ThreadIds.XY;
        int fIndex = (index2D.Y * DispatchSize.X) + index2D.X;

        var ray = rayBuffer[fIndex];
        
        // Get the appropriate dump value
        float4 value; 
        switch (dumpType)
        {
            case 0:
                value = new float4(ray.origin, 1);
                break;
            default:
                value = new float4(ray.direction, 1);
                break;
        };

        renderBuffer[index2D] = value;
    }
}
