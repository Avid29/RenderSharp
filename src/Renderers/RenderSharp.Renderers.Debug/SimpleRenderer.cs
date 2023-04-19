// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using ComputeSharp;
using RenderSharp.Renderers.Debug.Shaders;
using RenderSharp.Rendering.Interfaces;
using RenderSharp.Scenes;
using RenderSharp.Utilities.Tiles;

namespace RenderSharp.Renderers.Debug;

/// <summary>
/// A simple naive renderer.
/// </summary>
public class SimpleRenderer : IRenderer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleRenderer"/> class.
    /// </summary>
    /// <param name="device">The <see cref="GraphicsDevice"/> to use for rendering.</param>
    public SimpleRenderer(GraphicsDevice device)
    {
        Device = device;
    }

    /// <inheritdoc/>
    public GraphicsDevice Device { get;  }
    
    /// <inheritdoc/>
    public IReadWriteNormalizedTexture2D<float4>? RenderBuffer { get; set; }
    
    /// <inheritdoc/>
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
