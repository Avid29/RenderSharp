// Adam Dernis 2023

using RenderSharp.Scenes;
using RenderSharp.Scenes.Geometry.Meshes;
using System.Collections.Generic;

namespace RenderSharp.ImportExport;

public class ImportResult
{
    public ImportResult()
    {
        Objects = new List<Object>();
        Meshes = new List<Mesh>();
    }

    public List<Object> Objects { get; }

    public List<Mesh> Meshes { get; }

    internal ImportResult Merge(ImportResult mergeResult)
    {
        this.Objects.AddRange(mergeResult.Objects);
        this.Meshes.AddRange(mergeResult.Meshes);
        return this;
    }
}
