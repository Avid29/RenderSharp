// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Shaders.Pipeline.Collision.Interfaces;

namespace RenderSharp.RayTracing.Shaders.Pipeline.Collision;

/// <summary>
/// A class for running <see cref="ICollisionShader"/>s.
/// </summary>
/// <typeparam name="T">The <see cref="ICollisionShader"/> type.</typeparam>
public class CollisionShaderRunner<T> : CollisionShaderRunner
    where T : struct, ICollisionShader
{
    private readonly T _shader;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="CollisionShaderRunner{T}"/> class.
    /// </summary>
    /// <param name="shader">The shader to run.</param>
    public CollisionShaderRunner(T shader)
    {
        _shader = shader;
    }

    /// <inheritdoc/>
    public override void Enqueue(in ComputeContext context, int count)
    {
        context.For(count, _shader);
    }
}
