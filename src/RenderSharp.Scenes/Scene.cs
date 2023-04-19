// Adam Dernis 2023

using RenderSharp.Scenes.Cameras;
using RenderSharp.Scenes.Geometry;
using RenderSharp.Scenes.Lights;
using System.Collections.Generic;

namespace RenderSharp.Scenes;

/// <summary>
/// A class for a common RenderSharp scene object.
/// </summary>
public class Scene
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Scene"/> class.
    /// </summary>
    /// <param name="activeCamera">The scene's active camera.</param>
    public Scene(Camera activeCamera)
    {
        ActiveActiveCamera = activeCamera;
        Geometry = new List<GeometryObject>();
        Lights = new List<LightSource>();
    }

    /// <summary>
    /// Gets or sets the scene's active camera.
    /// </summary>
    public Camera ActiveActiveCamera { get; set; }

    /// <summary>
    /// Gets or sets the list of <see cref="LightSource"/>s in the scene.
    /// </summary>
    public List<LightSource> Lights { get; set; }

    /// <summary>
    /// Gets or sets the list of <see cref="GeometryObject"/>s in the scene.
    /// </summary>
    public List<GeometryObject> Geometry { get; set; }
}
