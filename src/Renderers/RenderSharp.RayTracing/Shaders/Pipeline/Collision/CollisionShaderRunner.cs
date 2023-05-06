// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Shaders.Pipeline.Collision.Interfaces;

namespace RenderSharp.RayTracing.Shaders.Pipeline.Collision;

/// <summary>
/// A base class for running collision shaders.
/// </summary>
public abstract class CollisionShaderRunner
{
    /// <summary>
    /// Enqueues an <see cref="ICollisionShader"/> to run in a <see cref="ComputeContext"/>.
    /// </summary>
    /// <param name="context">The <see cref="ComputeContext"/> to run in.</param>
    /// <param name="count">The number of collisions to run.</param>
    public abstract void Enqueue(in ComputeContext context, int count);
}
