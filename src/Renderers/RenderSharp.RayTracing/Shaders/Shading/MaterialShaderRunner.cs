// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.Utilities.Tiles;

namespace RenderSharp.RayTracing.Shaders.Shading;

public abstract class MaterialShaderRunner
{
    public abstract void Enqueue(in ComputeContext context, Tile tile);
}
