// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using RenderSharp.Scenes.Geometry.Meshes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text.RegularExpressions;

namespace RenderSharp.ImportExport.WaveFront;

public partial class WaveFrontImporter
{
    private readonly Dictionary<string, Action<string>> _actionDictionary;

    private WaveFrontImporter()
    {
        _currentMesh = new Mesh();
        _vertices = new List<Vector3>();
        _vertexNormals = new List<Vector3>();
        _meshes = new List<Mesh>();

        _actionDictionary = new()
        {
            {"o", ParseObjectLine},
            {"f", ParseFaceLine},
            {"v", ParseVertexLine},
        };
    }

    public static ImportResult Parse(string objFile, string? mtlFile = null, bool useMtl = true)
    {
        var importer = new WaveFrontImporter();
        importer.ParseFile(objFile);

        if (useMtl)
        {
            mtlFile ??= MtlFromObjFile(objFile);

            Guard.IsNotNull(mtlFile);

            importer.ParseFile(mtlFile);
        }

        return importer.CreateResult();
    }

    [GeneratedRegex(@"([\.\w]*)(\.\w*)")]
    private static partial Regex FileNameRegex();

    private static string? MtlFromObjFile(string file)
    {
        var match = FileNameRegex().Match(file);
        if (!match.Success)
            return null;

        var rawName = match.Groups[1].Value;
        return rawName + ".mtl";
    }

    private void ParseFile(string filename)
    {
        using var stream = File.OpenText(filename);

        while (!stream.EndOfStream)
        {
            var line = stream.ReadLine();
            if (line is null)
                return;

            var trim = line.TrimStart();
            if (trim[0] == '#')
                continue;
            var code = trim[..line.IndexOf(' ')];

            if (_actionDictionary.ContainsKey(code))
                _actionDictionary[code](line);
        }
    }

    private ImportResult CreateResult()
    {
        var result = new ImportResult();
        foreach (var mesh in _meshes)
        {
            result.Meshes.Add(mesh);
        }

        return result;
    }
}
