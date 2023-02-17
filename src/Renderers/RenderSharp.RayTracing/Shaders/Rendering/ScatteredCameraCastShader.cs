﻿// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Models;
using RenderSharp.RayTracing.Models.Camera;
using RenderSharp.RayTracing.Models.Rays;
using RenderSharp.Utilities.Tiles;

namespace RenderSharp.RayTracing.Shaders.Rendering;

[AutoConstructor]
[EmbeddedBytecode(DispatchAxis.XY)]
public partial struct ScatteredCameraCastShader : IComputeShader
{
    private readonly Tile tile;
    private readonly int2 imageSize;
    private readonly Camera camera;
    private readonly ReadWriteBuffer<Ray> rayBuffer;
    private readonly ReadWriteBuffer<Rand> randBuffer;

    public void Execute()
    {
        // Get the index of resources managed by the current thread
        // in both 2D textures and flat buffers
        int2 index2D = ThreadIds.XY;
        int fIndex = (index2D.Y * DispatchSize.X) + index2D.X;
        int2 imageIndex = index2D + tile.offset;

        Rand rand = randBuffer[fIndex];

        float uOffset = rand.NextFloat();
        float vOffset = rand.NextFloat();

        // Calculate the camera u and v normalized pixel coordinates.
        float u = (imageIndex.X + uOffset) / imageSize.X;
        float v = 1 - (imageIndex.Y + vOffset) / imageSize.Y;

        // Create a ray from the camera and store it in the ray buffer.
        var ray = Camera.CreateRay(camera, u, v, ref rand);
        randBuffer[fIndex] = rand;
        rayBuffer[fIndex] = ray;
    }
}
