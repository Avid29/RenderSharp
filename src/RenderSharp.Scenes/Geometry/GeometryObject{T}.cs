// Adam Dernis 2023

using RenderSharp.Scenes.Geometry.Interfaces;
using RenderSharp.Scenes.Geometry.Meshes;

namespace RenderSharp.Scenes.Geometry;

public class GeometryObject<T> : GeometryObject
    where T : IGeometry
{
    public GeometryObject(T geometry)
    {
        Geometry = geometry;
    }

    public T Geometry { get; set; }

    // TODO: Apply transformations!
    public override Mesh ConvertToMesh()
        => Geometry.ConvertToMesh();
}
