// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using RenderSharp.Scenes;
using RenderSharp.Scenes.Geometry.Meshes;
using System.Collections.Generic;
using System.Numerics;
using System.Text.RegularExpressions;

namespace RenderSharp.ImportExport.WaveFront;

public partial class WaveFrontImporter
{
    private readonly List<Vector3> _vertices;
    private List<Vector3> _vertexNormals;
    private List<Mesh> _meshes;
    private Mesh _currentMesh;
    
    [GeneratedRegex(@"^o \w+$")]
    private static partial Regex ObjectLineRegex();

    public void ParseObjectLine(string line)
    {
        var match = ObjectLineRegex().Match(line);
        
        if (!match.Success)
            return;

        if (_currentMesh.Faces.Count == 0)
            return;

        _currentMesh = new Mesh();
    }

    [GeneratedRegex(@"^f([ ]+\d+(?:/\d*(?:/\d+)?)?){3,}$")]
    private static partial Regex FaceLineRegex();

    public void ParseFaceLine(string line)
    {
        var match = FaceLineRegex().Match(line);

        if (!match.Success)
            return;

        var vertices = new List<Vertex>();

        for (int i = 1; i < match.Groups.Count; i++)
        {
            var vertex = ParseFaceVertex(match.Groups[i].Value);

            if (vertex is not null)
            {
                vertices.Add(vertex);
                _currentMesh.Vertices.Add(vertex);
            }
        }

        _currentMesh.Faces.Add(new Face(vertices[0], vertices[1], vertices[2]));
    }
    
    [GeneratedRegex(@"^(\d+)(?:/(\d*))?(?:/(\d+))?$")]
    private static partial Regex FaceVertexRegex();

    public Vertex? ParseFaceVertex(string vertex)
    {
        var match = FaceVertexRegex().Match(vertex);

        if (!match.Success)
            return null;

        int v = int.Parse(match.Groups[1].Value);
        int vn = 0;

        //if (match.Groups.Count >= 2)
        //    vt = int.Parse(match.Groups[2].Value);

        if (match.Groups.Count >= 3)
            vn = int.Parse(match.Groups[3].Value);

        var vector = _vertices[v-1];
        // TODO: Vertex Textures
        var vertexNormal = _vertexNormals[vn-1];

        return new Vertex(vector, vertexNormal);
    }
    
    [GeneratedRegex(@"^v(?:[ ]+([+-]?(?:[0-9]*[.])?[0-9]+)){3,4}$")]
    private static partial Regex VertexLineRegex();

    public void ParseVertexLine(string line)
    {
        var match = VertexLineRegex().Match(line);

        if (!match.Success)
            return;

        Guard.IsBetweenOrEqualTo(match.Groups.Count, 4, 5);
        
        // TODO: Figure out what to do with w
        float w = 1f;
        var x = float.Parse(match.Groups[1].Value);
        var y = float.Parse(match.Groups[2].Value);
        var z = float.Parse(match.Groups[3].Value);
        if (match.Groups.Count is 5)
            w = float.Parse(match.Groups[4].Value);

        var vector = new Vector3(x, y, z);
        _vertices.Add(vector);
    }
}
