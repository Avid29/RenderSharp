// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Models.Camera;

namespace RenderSharp.RayTracing.Shaders.Debugging;

/// <summary>
/// An <see cref="IComputeShader"/> that renders a BVH Tree's bounding boxes to the render buffer.
/// </summary>
[AutoConstructor]
[EmbeddedBytecode(DispatchAxis.X)]
public readonly partial struct BVHRenderingShader : IComputeShader
{
    private readonly IReadWriteNormalizedTexture2D<float4> renderBuffer;
    private readonly PinholeCamera camera;
    
    /// <inheritdoc/>
    public void Execute()
    {
        // TODO: Draw AABBs to render buffer
    }
}
