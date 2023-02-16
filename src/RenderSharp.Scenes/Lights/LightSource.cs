// Adam Dernis 2023

using System.Numerics;

namespace RenderSharp.Scenes.Lights;

public abstract class LightSource : Object
{
    public Vector3 Color { get; set; }

    public float Power { get; set; }
}
