// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.Scenes;
using RenderSharp.Rendering.Base;
using RenderSharp.Utilities.Tiles;

namespace RenderSharp.Rendering.Interfaces;

/// <summary>
/// An interface for rendering with the <see cref="RenderManager"/>.
/// </summary>
public interface IRenderer : IRenderingComponent
{
    /// <summary>
    /// Gets or sets the render buffer to be used by the renderer.
    /// </summary>
    IReadWriteNormalizedTexture2D<float4>? RenderBuffer { get; set; }

    /// <summary>
    /// Loads the 3D scene to the renderer.
    /// </summary>
    /// <param name="scene">The common RenderSharp scene to load.</param>
    void SetupScene(Scene scene);

    /// <summary>
    /// Renders the full image to the buffer.
    /// </summary>
    void Render();

    /// <summary>
    /// Renders a segment of the image to the buffer.
    /// </summary>
    /// <param name="tile">The tile of the image to render.</param>
    void RenderSegment(Tile tile);
}
