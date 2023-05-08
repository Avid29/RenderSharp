// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Shaders.Pipeline.Collision.Interfaces;

namespace RenderSharp.RayTracing.Shaders.Pipeline.RayCasting.Camera;

/// <summary>
/// A base for running camera cast shaders.
/// </summary>
public abstract class CameraCastShaderRunner
{
    /// <summary>
    /// Enqueues an <see cref="ICollisionShader"/> to run in a <see cref="ComputeContext"/>.
    /// </summary>
    public abstract void Enqueue(in ComputeContext context, int x, int y);
}
