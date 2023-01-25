// Adam Dernis 2023

using System;
using ComputeSharp;
using ComputeSharp.WinUI;
using RenderSharp.Rendering;

namespace RenderSharp.UI.Shared.Rendering;

public class RenderViewer<TRenderer> : IShaderRunner
    where TRenderer : IRenderer
{
    private readonly RenderManager<TRenderer> _renderManager;

    public RenderViewer(TRenderer renderer)
    {
        _renderManager = new RenderManager<TRenderer>(renderer);
    }

    public void Setup()
    {
        _renderManager.LoadScene();
    }

    public void Refresh()
    {
        _renderManager.CancelRendering();
    }

    public bool TryExecute(IReadWriteNormalizedTexture2D<float4> texture, TimeSpan timespan, object? parameter)
    {;
        // Start rendering if the renderer is ready
        if (_renderManager.IsReady)
        {
            _renderManager.BeginRendering(texture.Width, texture.Height);
        }

        // If the renderer is neither running nor done, return false
        if (!_renderManager.IsRunning && !_renderManager.IsDone)
            return false;

        // Render the most current frame
        _renderManager.RenderFrame(texture);
        return true;

    }
}
