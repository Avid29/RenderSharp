// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Scene.Geometry;
using RenderSharp.RayTracing.Scene.Rays;
using RenderSharp.RayTracing.Utils;

namespace RenderSharp.RayTracing.Shaders.Debugging;

[AutoConstructor]
[EmbeddedBytecode(DispatchAxis.XY)]
public readonly partial struct RayCastBufferDumpShader : IComputeShader
{
    private readonly ReadWriteBuffer<RayCast> rayCastBuffer;
    private readonly ReadOnlyBuffer<Triangle> geometryBuffer;
    private readonly IReadWriteNormalizedTexture2D<float4> renderBuffer;
    private readonly int objectCount;

    /// <remarks>
    /// TODO: Use enum
    /// Waiting on https://github.com/Sergio0694/ComputeSharp/issues/248
    /// </remarks>
    private readonly int dumpType;

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
            default:
                value = new float4(rayCast.position, 1);
                break;
            case 1:
                value = new float4(rayCast.normal, 1);
                break;
            case 2:
                value = new float4(rayCast.distance, 0, 0, 1);
                break;
            case 3:
            case 4:
                int id = rayCast.triId;
                int count = geometryBuffer.Length;

                if (dumpType == 4)
                {
                    id = geometryBuffer[rayCast.triId].objId;
                    count = objectCount;
                }

                float hue = (id * 360f) / (count * 1.05f);
                var hsv = new float3(hue, 1f, rayCast.triId == -1 ? 0 : 1);
                value = new float4(VectorUtils.HSVtoRGB(hsv), 1);
                break;
        };

        renderBuffer[index2D] = value;
    }
}
