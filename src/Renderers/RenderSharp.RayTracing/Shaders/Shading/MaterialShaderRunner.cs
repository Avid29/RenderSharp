// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.Utilities.Tiles;
using RenderSharp.RayTracing.Shaders.Shading.Interfaces;
using RenderSharp.RayTracing.Utils;

namespace RenderSharp.RayTracing.Shaders.Shading;

/// <summary>
/// A base class for initializing and running <see cref="IMaterialShader{TMat}"/>s.
/// </summary>
public abstract class MaterialShaderRunner
{
    /// <summary>
    /// Sets the buffers in a shader.
    /// </summary>
    internal abstract void SetShaderBuffers(BufferCollection buffers);

    /// <summary>
    /// Enqueues a shader to run in a <see cref="ComputeContext"/>.
    /// </summary>
    /// <param name="context">The <see cref="ComputeContext"/> to run in.</param>
    /// <param name="tile">The tile data to render.</param>
    internal abstract void Enqueue(in ComputeContext context, Tile tile);
}
