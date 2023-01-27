// Adam Dernis 2023

using RenderSharp.Scenes.Cameras;
using System.Collections.Generic;

namespace RenderSharp.Scenes;

public class Scene
{
    public Scene(Camera camera)
    {
        ActiveCamera = camera;
        Objects = new List<Object>();
    }

    public Camera ActiveCamera { get; set; }

    public List<Object> Objects { get; set; }
}
