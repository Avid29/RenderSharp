// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Models;
using RenderSharp.RayTracing.Models.Camera;
using RenderSharp.RayTracing.RayCasts;
using RenderSharp.Utilities.Tiles;

namespace RenderSharp.RayTracing.Shaders.Pipeline;

[AutoConstructor]
[EmbeddedBytecode(DispatchAxis.XY)]
public partial struct ScatteredCameraCastShader : IComputeShader
{
    private readonly Tile tile;
    private readonly int2 imageSize;
    private readonly PinholeCamera camera;
    private readonly ReadWriteBuffer<Ray> rayBuffer;
    private readonly ReadWriteBuffer<Rand> randBuffer;
    private int sample;
    private int samplesSqrt;

    public void Execute()
    {
        // Get the index of resources managed by the current thread
        // in both 2D textures and flat buffers
        int2 index2D = ThreadIds.XY;
        int fIndex = (index2D.Y * DispatchSize.X) + index2D.X;
        int2 imageIndex = index2D + tile.offset;

        Rand rand = randBuffer[fIndex];

        //int row = sample % samplesSqrt;
        //int col = sample / samplesSqrt;

        //int sSqrt1 = samplesSqrt + 1;
        //float uOffset = (float)row / sSqrt1;
        //float vOffset = (float)col / sSqrt1;

        float uOffset = rand.NextFloat();
        float vOffset = rand.NextFloat();

        // Calculate the camera u and v normalized pixel coordinates.
        float u = (imageIndex.X + uOffset) / imageSize.X;
        float v = 1 - (imageIndex.Y + vOffset) / imageSize.Y;

        // Create a ray from the camera and store it in the ray buffer.
        var ray = PinholeCamera.CreateRay(camera, u, v);
        randBuffer[fIndex] = rand;
        rayBuffer[fIndex] = ray;
    }
}
