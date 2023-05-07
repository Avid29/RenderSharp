// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.Rendering.Manager.Base;

namespace RenderSharp.Rendering.Interfaces;

/// <summary>
/// An interface for post processing with the <see cref="RenderManagerBase"/>
/// </summary>
public interface IPostProcessor : IRenderingComponent
{
    /// <summary>
    /// Run tone reproduction on the buffer.
    /// </summary>
    /// <param name="buffer">The buffer to post process.</param>
    void Process(IReadWriteNormalizedTexture2D<float4> buffer);
}
