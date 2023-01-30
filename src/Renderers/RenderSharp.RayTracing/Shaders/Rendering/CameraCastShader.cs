// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Scene.Camera;
using RenderSharp.RayTracing.Scene.Rays;
using RenderSharp.Utilities.Tiles;

namespace RenderSharp.RayTracing.Shaders.Rendering;

/// <summary>
/// An <see cref="IComputeShader"/> that creates camera rays.
/// </summary>
[AutoConstructor]
[EmbeddedBytecode(DispatchAxis.XY)]
public readonly partial struct CameraCastShader : IComputeShader
{
    private readonly Tile tile;
    private readonly int2 imageSize;
    private readonly Camera camera;
    private readonly ReadWriteBuffer<Ray> rayBuffer;

    public void Execute()
    {
        // Get the index of resources managed by the current thread
        // in both 2D textures and flat buffers
        int2 index2D = ThreadIds.XY;
        int fIndex = (index2D.Y * DispatchSize.X) + index2D.X;
        int2 imageIndex = index2D + tile.offset;

        // Calculate the camera u and v normalized pixel coordinates.
        float u = (imageIndex.X + 0.5f) / imageSize.X;
        float v = 1 - (imageIndex.Y + 0.5f) / imageSize.Y;

        // Create a ray from the camera and store it in the ray buffer.
        var ray = Camera.CreateRay(camera, u, v);
        rayBuffer[fIndex] = ray;
    }
}
