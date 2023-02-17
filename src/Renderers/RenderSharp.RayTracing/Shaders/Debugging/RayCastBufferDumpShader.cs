// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Models.Geometry;
using RenderSharp.RayTracing.Models.Rays;
using RenderSharp.RayTracing.Utils;
using RenderSharp.Utilities.Tiles;

namespace RenderSharp.RayTracing.Shaders.Debugging;

/// <summary>
/// An <see cref="IComputeShader"/> that dumps a property from the <see cref="GeometryCollision"/> buffer as a color.
/// </summary>
[AutoConstructor]
[EmbeddedBytecode(DispatchAxis.XY)]
public readonly partial struct RayCastBufferDumpShader : IComputeShader
{
    private readonly Tile tile;
    private readonly ReadWriteBuffer<GeometryCollision> rayCastBuffer;
    private readonly ReadOnlyBuffer<Triangle> geometryBuffer;
    private readonly IReadWriteNormalizedTexture2D<float4> renderBuffer;
    private readonly int objectCount;

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
        int2 imageIndex = index2D + tile.offset;

        var rayCast = rayCastBuffer[fIndex];
        
        // Get the appropriate dump value
        float4 value; 
        switch (dumpType)
        {
            default:
                value = new float4(Hlsl.Abs(rayCast.position), 1);
                break;
            case 1:
                value = new float4(Hlsl.Abs(Hlsl.Normalize(rayCast.normal)), 1);
                break;
            case 2:
                value = new float4(rayCast.distance, 0, 0, 1);
                break;
            case 3:
            case 4:
                int id = rayCast.geoId;
                int count = geometryBuffer.Length;

                // Override the ID and count to the object id and count if dumping object data
                if (dumpType == 4)
                {
                    id = geometryBuffer[rayCast.geoId].objId;
                    count = objectCount;
                }

                float hue = (id * 360f) / (count * 1.05f);
                var hsv = new float3(hue, 1f, rayCast.geoId == -1 ? 0 : 1);
                value = new float4(VectorUtils.HSVtoRGB(hsv), 1);
                break;
        };

        renderBuffer[imageIndex] = value;
    }
}
