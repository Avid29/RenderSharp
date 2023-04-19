// Adam Dernis 2023

using RenderSharp.Scenes;
using RenderSharp.Scenes.Geometry.Meshes;
using System.Collections.Generic;

namespace RenderSharp.ImportExport;

/// <summary>
/// Imported objects from an import operation.
/// </summary>
public class ImportResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ImportResult"/> class.
    /// </summary>
    public ImportResult()
    {
        Objects = new List<Object>();
        Meshes = new List<Mesh>();
    }

    /// <summary>
    /// Gets the list of imported objects.
    /// </summary>
    public List<Object> Objects { get; }

    /// <summary>
    /// Gets the list of imported meshes.
    /// </summary>
    public List<Mesh> Meshes { get; }

    internal ImportResult Merge(ImportResult mergeResult)
    {
        this.Objects.AddRange(mergeResult.Objects);
        this.Meshes.AddRange(mergeResult.Meshes);
        return this;
    }
}
