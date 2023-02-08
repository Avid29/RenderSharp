// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using ComputeSharp;
using RenderSharp.Renderers.Debug.Shaders;
using RenderSharp.Rendering.Interfaces;
using RenderSharp.Scenes;
using RenderSharp.Utilities.Tiles;

namespace RenderSharp.Renderers.Debug;

public class SimpleRenderer : IRenderer
{
    public SimpleRenderer(GraphicsDevice device)
    {
        Device = device;
    }

    public GraphicsDevice Device { get; set; }
    
    public IReadWriteNormalizedTexture2D<float4>? RenderBuffer { get; set; }

    public IRenderAnalyzer? RenderAnalyzer { get; set; }

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
    public void RenderSegment(Tile tile)
    {
        Guard.IsNotNull(RenderBuffer);
    }
}
