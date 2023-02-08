﻿// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using ComputeSharp;
using ComputeSharp.WinUI;
using RenderSharp.ImportExport.WaveFront;
using RenderSharp.Rendering.Base;
using RenderSharp.Rendering.Interfaces;
using RenderSharp.Scenes;
using RenderSharp.Scenes.Cameras;
using RenderSharp.Scenes.Geometry;
using RenderSharp.Scenes.Geometry.Tessellation.Shapes;
using System;
using System.Numerics;

using Plane = RenderSharp.Scenes.Geometry.Tessellation.Shapes.Plane;

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
        //var import = WaveFrontImporter.Parse(@"C:\Users\avid2\source\repos\Personal\RenderSharp\samples\RenderSharp.Samples.WinUI\Assets\test.obj");

        //var camera = Camera.CreateFromLookAt(new Vector3(0f, 5f, 0f), new Vector3(0, 0f, 0f), 75);
        var camera = Camera.CreateFromEuler(new Vector3(0f, 1f, 0f), new Vector3(0f, 180f, 0f), 75);
        var scene = new Scene(camera);

        //scene.Objects.AddRange(import.Objects);

        scene.Objects.AddRange(new GeometryObject[]
        {
            new GeometryObject<UVSphere>(new UVSphere { Radius = 0.5f }) { Transformation = Transformation.CreateFromTranslation(new Vector3(0, 1, 2)) },
            new GeometryObject<UVSphere>(new UVSphere { Radius = 0.4f }) { Transformation = Transformation.CreateFromTranslation(new Vector3(-0.75f, 0.5f, 2.5f)) },
            new GeometryObject<Plane>(new Plane { Size = 1 })
            {
                Transformation = new Transformation
                {
                    Translation = new Vector3(-1.25f, 0, 4),
                    Scale = new Vector3(5.5f, 1, 8),
                }
            },
        });

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
        return _renderManager.RenderFrame(texture);
    }
}
