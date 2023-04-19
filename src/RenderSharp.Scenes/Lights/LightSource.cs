// Adam Dernis 2023

using System.Numerics;

namespace RenderSharp.Scenes.Lights;

/// <summary>
/// A base class for light sources for the RenderSharp common scene.
/// </summary>
public abstract class LightSource : Object
{
    /// <summary>
    /// Gets or sets the color of the light.
    /// </summary>
    public Vector3 Color { get; set; }

    /// <summary>
    /// Gets or sets the intensity of the light source.
    /// </summary>
    public float Power { get; set; }
}
