// Adam Dernis 2023

using ComputeSharp;

namespace RenderSharp.Rendering;

public interface IRenderer
{
    /// <summary>
    /// The device the renderer is using to render.
    /// </summary>
    GraphicsDevice Device { get; }

    /// <summary>
    /// Gets or sets the render buffer to be used by the renderer.
    /// </summary>
    IReadWriteNormalizedTexture2D<float4>? RenderBuffer { get; set; }

    /// <summary>
    /// Loads the 3D scene to the renderer.
    /// </summary>
    void SetupScene();

    /// <summary>
    /// Renders the full image to the buffer.
    /// </summary>
    void Render();

    /// <summary>
    /// Renders a segment of the image to the buffer.
    /// </summary>
    /// <param name="offset">The offset of the segment.</param>
    /// <param name="size">The size of the segment.</param>
    void RenderSegment(int2 offset, int2 size);
}
