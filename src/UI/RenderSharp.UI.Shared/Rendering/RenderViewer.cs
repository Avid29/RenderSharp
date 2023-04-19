// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using ComputeSharp;
using ComputeSharp.WinUI;
using RenderSharp.ImportExport.WaveFront;
using RenderSharp.Rendering.Base;
using RenderSharp.Rendering.Interfaces;
using RenderSharp.Scenes;
using RenderSharp.Scenes.Cameras;
using RenderSharp.Scenes.Geometry;
using RenderSharp.Scenes.Lights;
using System;
using System.Linq;
using System.Numerics;

namespace RenderSharp.UI.Shared.Rendering;

/// <summary>
/// A class for running an <see cref="IRenderer"/> through a <see cref="RenderManager"/>.
/// </summary>
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
        var import = WaveFrontImporter.Parse(@"C:\Users\avid2\source\repos\Personal\RenderSharp\samples\RenderSharp.Samples.WinUI\Assets\Scene-FullSphere.obj");

        //var camera = Camera.CreateFromLookAt(new Vector3(0f, 5f, 0f), new Vector3(0, 0f, 0f), 75);
        var camera = Camera.CreateFromEuler(new Vector3(0f, 1f, 0f), new Vector3(0f, 180f, 0f), 75);
        var scene = new Scene(camera);

        scene.Geometry.AddRange(import.Objects.OfType<GeometryObject>());

        scene.Lights.AddRange(new LightSource[]
        {
            new PointLight
            {
                Color = Vector3.One,
                Power = 0.5f,
                Radius = 0.25f,
                Transformation = Transformation.CreateFromTranslation(new Vector3(0.5f, 3.5f, 0.5f)),
            },
            //new PointLight
            //{
            //    Color = Vector3.One,
            //    Power = 0.2f,
            //    Radius = 0.25f,
            //    Transformation = Transformation.CreateFromTranslation(new Vector3(-1.5f, 3.5f, 1f)),
            //},
        });

        _renderManager = new TManager();
        _renderManager.Renderer = renderer;
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
