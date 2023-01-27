// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Scene.Rays;

namespace RenderSharp.RayTracing.Shaders.Debugging;

[AutoConstructor]
[EmbeddedBytecode(DispatchAxis.XY)]
public readonly partial struct RayCastBufferDumpShader : IComputeShader
{
    private readonly ReadWriteBuffer<RayCast> rayCastBuffer;
    private readonly IReadWriteNormalizedTexture2D<float4> renderBuffer;

    /// <remarks>
    /// TODO: Use enum
    /// Waiting on https://github.com/Sergio0694/ComputeSharp/issues/248
    /// </remarks>
    private readonly int dumpType;
    private readonly int objectCount;

    public void Execute()
    {
        // Get the index of resources managed by the current thread
        // in both 2D textures and flat buffers
        int2 index2D = ThreadIds.XY;
        int fIndex = (index2D.Y * DispatchSize.X) + index2D.X;

        var rayCast = rayCastBuffer[fIndex];
        
        // Get the appropriate dump value
        float4 value; 
        switch (dumpType)
        {
            case 0:
                value = new float4(rayCast.position, 1);
                break;
            case 1:
                value = new float4(rayCast.normal, 1);
                break;
            case 2:
                value = new float4(rayCast.distance, 0, 0, 1);
                break;
            default:
                var i = (float)(rayCast.objId + 1) / objectCount;
                value = new float4((float3)i, 1);
                break;
        };

        renderBuffer[index2D] = value;
    }
}
