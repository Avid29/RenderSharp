// Adam Dernis 2023

using RenderSharp.Scenes.Geometry.Interfaces;
using RenderSharp.Scenes.Geometry.Meshes;
using System;

namespace RenderSharp.Scenes.Geometry.Tessellation;
public abstract class TessellatedShape : IGeometry
{
    public void ApplyTransformation(Transformation transformation)
    {
        throw new NotImplementedException();
    }

    public abstract Mesh ConvertToMesh();
}
