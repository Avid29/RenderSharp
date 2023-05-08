// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Shaders.Pipeline.RayCasting.Camera.Interfaces;

namespace RenderSharp.RayTracing.Shaders.Pipeline.RayCasting.Camera;

/// <summary>
/// A class for running <see cref="ICameraCastShader"/>s.
/// </summary>
/// <typeparam name="T">The <see cref="ICameraCastShader"/> type.</typeparam>
public class CameraCastShaderRunner<T> : CameraCastShaderRunner 
    where T : struct, ICameraCastShader
{
    private readonly T _shader;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="CameraCastShaderRunner{T}"/> class.
    /// </summary>
    /// <param name="shader">The shader to run.</param>
    public CameraCastShaderRunner(T shader)
    {
        _shader = shader;
    }

    /// <inheritdoc/>
    public override void Enqueue(in ComputeContext context, int x, int y)
    {
        context.For(x, y, _shader);
    }
}
