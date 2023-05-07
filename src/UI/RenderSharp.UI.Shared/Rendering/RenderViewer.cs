// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using ComputeSharp;
using ComputeSharp.WinUI;
using RenderSharp.Rendering.Interfaces;
using RenderSharp.Rendering.Manager.Base;
using RenderSharp.Scenes;
using System;

namespace RenderSharp.UI.Shared.Rendering;

/// <summary>
/// A class for running an <see cref="IRenderer"/> through a <see cref="RenderManagerBase"/>.
/// </summary>
public class RenderViewer : IShaderRunner
{
    private RenderManagerBase? _renderManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="RenderViewer"/> class.
    /// </summary>
    public RenderViewer()
    {
    }

    /// <summary>
    /// Sets up given a <typeparamref name="TManager"/> type, renderer, scene and optional post processor.
    /// </summary>
    public void Setup<TManager>(IRenderer renderer, Scene scene, IPostProcessor? postProcessor = null)
        where TManager : RenderManagerBase, new()
    {
        _renderManager = new TManager();
        _renderManager.Renderer = renderer;
        _renderManager.PostProcessor = postProcessor;
        _renderManager.LoadScene(scene);
    }

    /// <summary>
    /// Attaches a <see cref="RenderManagerBase"/> to the <see cref="RenderViewer"/>.
    /// </summary>
    /// <param name="renderManager"></param>
    public void Setup(RenderManagerBase renderManager)
    {
        _renderManager = renderManager;
    }

    /// <summary>
    /// Resets the <see cref="RenderManagerBase"/>.
    /// </summary>
    public void Refresh()
    {
        Guard.IsNotNull(_renderManager);

        _renderManager.Reset();
    }

    /// <inheritdoc/>
    public bool TryExecute(IReadWriteNormalizedTexture2D<float4> texture, TimeSpan timespan, object? parameter)
    {
        if (_renderManager is null)
            return false;

        // Start rendering if the renderer is ready
        if (_renderManager.IsReady)
        {
            _renderManager.Begin(texture.Width, texture.Height);
        }

        // If the renderer is neither running nor done, return false
        if (!_renderManager.IsRunning && !_renderManager.IsDone)
            return false;

        // Render the most current frame
        return _renderManager.FrameUpdate(texture);
    }
}
