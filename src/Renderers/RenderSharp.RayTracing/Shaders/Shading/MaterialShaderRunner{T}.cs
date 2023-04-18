// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Shaders.Shading.Interfaces;
using RenderSharp.Utilities.Tiles;

namespace RenderSharp.RayTracing.Shaders.Shading;

public class MaterialShaderRunner<T> : MaterialShaderRunner
    where T : struct, IMaterialShader
{
    private readonly T _shader;

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
        _shader.ColorBuffer = buffers.ColorBuffer;
    }

    public override void Enqueue(in ComputeContext context, Tile tile)
    {
        context.For(tile.Width, tile.Height, _shader);
    }
}
