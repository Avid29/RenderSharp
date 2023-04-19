// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.Utilities.Tiles;

namespace RenderSharp.RayTracing.Shaders.Shading;

/// <summary>
/// A base class for initializing and running material shaders.
/// </summary>
public abstract class MaterialShaderRunner
{
    /// <summary>
    /// Enqueues a shader to run in a <see cref="ComputeContext"/>.
    /// </summary>
    /// <param name="context">The <see cref="ComputeContext"/> to run in.</param>
    /// <param name="tile">The tile data to render.</param>
    public abstract void Enqueue(in ComputeContext context, Tile tile);
}
