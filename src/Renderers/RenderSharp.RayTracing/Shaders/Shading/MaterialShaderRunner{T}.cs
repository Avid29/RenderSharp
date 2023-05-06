// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Shaders.Shading.Interfaces;
using RenderSharp.RayTracing.Utils;
using RenderSharp.Utilities.Tiles;

namespace RenderSharp.RayTracing.Shaders.Shading;

/// <summary>
/// A class for initializing and running <see cref="IMaterialShader"/>s.
/// </summary>
/// <typeparam name="T">The <see cref="IMaterialShader"/> type.</typeparam>
public class MaterialShaderRunner<T> : MaterialShaderRunner
    where T : struct, IMaterialShader
{
    private readonly T _shader;

    /// <summary>
    /// Initializes a new instance of the <see cref="MaterialShaderRunner{T}"/> class.
    /// </summary>
    /// <param name="shader">The shader to run.</param>
    /// <param name="buffers">The buffer collection to initialize the shader with.</param>
    public MaterialShaderRunner(T shader, TileBufferCollection buffers)
    {
        _shader = shader;
        _shader.ObjectBuffer = buffers.ObjectBuffer;
        _shader.LightBuffer = buffers.LightBuffer;
        _shader.PathRayBuffer = buffers.PathRayBuffer;
        _shader.ShadowRayBuffer = buffers.ShadowRayBuffer;
        _shader.ShadowCastBuffer = buffers.ShadowCastBuffer;
        _shader.PathCastBuffer = buffers.PathCastBuffer;
        _shader.AttenuationBuffer = buffers.AttenuationBuffer;
        _shader.LuminanceBuffer = buffers.LuminanceBuffer;
    }

    /// <inheritdoc/>
    public override void Enqueue(in ComputeContext context, Tile tile)
    {
        context.For(tile.Width, tile.Height, _shader);
    }
}
