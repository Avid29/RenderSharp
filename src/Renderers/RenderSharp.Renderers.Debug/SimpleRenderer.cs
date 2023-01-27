// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using ComputeSharp;
using RenderSharp.Renderers.Debug.Shaders;
using RenderSharp.Rendering;
using RenderSharp.Scenes;

namespace RenderSharp.Renderers.Debug;

public class SimpleRenderer : IRenderer
{
    public SimpleRenderer(GraphicsDevice device)
    {
        Device = device;
    }

    public GraphicsDevice Device { get; set; }
    
    public IReadWriteNormalizedTexture2D<float4>? RenderBuffer { get; set; }
    
    /// <inheritdoc/>
    public void SetupScene(Scene scene)
    {
        // TODO: Load 3D scene
    }

    /// <inheritdoc/>
    public void Render()
    {
        Guard.IsNotNull(RenderBuffer);

        Device.For(RenderBuffer.Width, RenderBuffer.Height, new Shader(RenderBuffer));
    }
    
    /// <inheritdoc/>
    public void RenderSegment(int2 offset, int2 size)
    {
        Guard.IsNotNull(RenderBuffer);
    }
}
