// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using ComputeSharp;
using ComputeSharp.WinUI;
using RenderSharp.Rendering;
using RenderSharp.Scenes;
using RenderSharp.Scenes.Cameras;
using RenderSharp.Scenes.Geometry;
using RenderSharp.Scenes.Geometry.Tessellation.Shapes;
using System;
using System.Numerics;

namespace RenderSharp.UI.Shared.Rendering;

public class RenderViewer : IShaderRunner
{
    private RenderManager? _renderManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="RenderViewer"/> class.
    /// </summary>
    public RenderViewer()
    {
    }

    /// <summary>
    /// Sets up the <see cref="RenderManager"/>.
    /// </summary>
    public void Setup<TManager>(IRenderer renderer)
        where TManager : RenderManager, new()
    {
        var camera = Camera.CreateFromLookAt(new Vector3(5f, 5f, 5f), Vector3.Zero, 90);
        var scene = new Scene(camera);

        var cube = new GeometryObject<UVSphere>(new UVSphere());
        scene.Objects.Add(cube);

        _renderManager = new TManager();
        _renderManager.SetRenderer(renderer);
        _renderManager.LoadScene(scene);
    }

    /// <summary>
    /// Resets the <see cref="RenderManager"/>.
    /// </summary>
    public void Refresh()
    {
        Guard.IsNotNull(_renderManager);

        _renderManager.Reset();
    }

    /// <inheritdoc/>
    public bool TryExecute(IReadWriteNormalizedTexture2D<float4> texture, TimeSpan timespan, object? parameter)
    {
        Guard.IsNotNull(_renderManager);

        // Start rendering if the renderer is ready
        if (_renderManager.IsReady)
        {
            _renderManager.Begin(texture.Width, texture.Height);
        }

        // If the renderer is neither running nor done, return false
        if (!_renderManager.IsRunning && !_renderManager.IsDone)
            return false;

        // Render the most current frame
        return _renderManager.RenderFrame(texture);
    }
}
