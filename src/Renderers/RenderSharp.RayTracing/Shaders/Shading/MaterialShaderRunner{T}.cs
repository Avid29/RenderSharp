// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Shaders.Shading.Interfaces;
using RenderSharp.RayTracing.Utils;
using RenderSharp.Utilities.Tiles;

namespace RenderSharp.RayTracing.Shaders.Shading;

/// <summary>
/// A class for initializing and running <see cref="IMaterialShader{TMat}"/>s.
/// </summary>
/// <typeparam name="T">The <see cref="IMaterialShader{TMat}"/> type.</typeparam>
/// <typeparam name="TMat">The <see cref="IMaterialShader{TMat}"/> config.</typeparam>
internal class MaterialShaderRunner<T, TMat> : MaterialShaderRunner
    where T : struct, IMaterialShader<TMat>
    where TMat : struct
{
    private T _shader;

    /// <summary>
    /// Initializes a new instance of the <see cref="MaterialShaderRunner{T, TMat}"/> class.
    /// </summary>
    internal MaterialShaderRunner(int id, TMat mat)
    {
        _shader = new()
        {
            MaterialId = id,
            Material = mat
        };
    }
    
    /// <inheritdoc/>
    internal override void SetShaderBuffers(BufferCollection buffers)
    {
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
    internal override void Enqueue(in ComputeContext context, Tile tile)
    {
        context.For(tile.Width, tile.Height, _shader);
    }
}
