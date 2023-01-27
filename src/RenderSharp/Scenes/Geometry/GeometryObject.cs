// Adam Dernis 2023

using RenderSharp.Scenes.Geometry.Meshes;

namespace RenderSharp.Scenes.Geometry;
public abstract class GeometryObject : Object
{
    public abstract Mesh ConvertToMesh();
}
