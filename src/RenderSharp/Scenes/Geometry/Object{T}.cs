// Adam Dernis 2023

using RenderSharp.Scenes.Geometry.Interfaces;

namespace RenderSharp.Scenes.Geometry;

public class GeometryObject<T> : Object
    where T : IGeometry
{
    public GeometryObject(T? geometry)
    {
        Geometry = geometry;
    }

    public T? Geometry { get; set; }
}
