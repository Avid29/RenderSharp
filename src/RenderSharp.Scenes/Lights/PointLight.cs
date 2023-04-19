// Adam Dernis 2023

namespace RenderSharp.Scenes.Lights;

/// <summary>
/// A class for point lights for the common RenderSharp scene.
/// </summary>
public class PointLight : LightSource
{
    /// <summary>
    /// Gets or sets the radius of the point light.
    /// </summary>
    public float Radius { get; set; }
}
