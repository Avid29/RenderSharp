// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.Rendering.Analyzer.Interfaces;

namespace RenderSharp.Rendering.Interfaces;

/// <summary>
/// An interface for components used for rendering.
/// </summary>
public interface IRenderingComponent
{
    /// <summary>
    /// The device the renderer is using to render.
    /// </summary>
    GraphicsDevice? Device { get; set; }

    /// <summary>
    /// Gets or sets the render analyzer for tracking render progress and time.
    /// </summary>
    IRenderAnalyzer? RenderAnalyzer { get; set; }
}
