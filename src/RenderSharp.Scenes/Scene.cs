// Adam Dernis 2023

using RenderSharp.Scenes.Cameras;
using RenderSharp.Scenes.Geometry;
using RenderSharp.Scenes.Lights;
using System.Collections.Generic;

namespace RenderSharp.Scenes;

public class Scene
{
    public Scene(Camera camera)
    {
        ActiveCamera = camera;
        Geometry = new List<GeometryObject>();
        Lights = new List<LightSource>();
    }

    public Camera ActiveCamera { get; set; }

    public List<LightSource> Lights { get; set; }

    public List<GeometryObject> Geometry { get; set; }
}
