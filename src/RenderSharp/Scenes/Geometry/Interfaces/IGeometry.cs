// Adam Dernis 2023

using RenderSharp.Scenes.Geometry.Meshes;

namespace RenderSharp.Scenes.Geometry.Interfaces;

public interface IGeometry
{
    Mesh ConvertToMesh();

    void ApplyTransformation(Transformation transformation);
}
