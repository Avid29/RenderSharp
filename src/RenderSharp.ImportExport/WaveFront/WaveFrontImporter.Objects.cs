// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using RenderSharp.Scenes.Geometry;
using RenderSharp.Scenes.Geometry.Meshes;
using System.Collections.Generic;
using System.Numerics;
using System.Text.RegularExpressions;

namespace RenderSharp.ImportExport.WaveFront;

public partial class WaveFrontImporter
{
    private readonly List<Vector3> _vertices;
    private readonly List<Vector3> _vertexNormals;
    private readonly List<GeometryObject<Mesh>> _meshes;
    private GeometryObject<Mesh> _currentMesh;
    

    public void ParseObjectLine(string line)
    {
        var split = line.Split(' ');

        if (split.Length != 2)
            return;

        FinishObject();

        _currentMesh = new GeometryObject<Mesh>(new Mesh()){Name = split[1]};
    }

    public void ParseFaceLine(string line)
    {
        var split = line.Split(' ');

        if (split.Length < 3)
            return;

        var vertices = new List<Vertex>();

        for (int i = 1; i < split.Length; i++)
        {
            var vertex = ParseFaceVertex(split[i]);

            if (vertex is not null)
            {
                vertices.Add(vertex);
                _currentMesh.Geometry.Vertices.Add(vertex);
            }
        }

        _currentMesh.Geometry.Faces.Add(new Face(vertices[0], vertices[1], vertices[2]));
    }

    public Vertex? ParseFaceVertex(string vertex)
    {
        var split = vertex.Split('/');

        if (split.Length is < 1 or > 3)
            return null;

        int v = int.Parse(split[0]);
        var vector = _vertices[v-1];

        // TODO: Vertex Textures
        //if (split.Length > 2)
        //{
        //    int vt = int.Parse(split[1]);
        //}

        // TODO: Vertex Normals
        //if (split.Length > 3)
        //{
        //    int vn = int.Parse(split[2]);
        //    var vertexNormal = _vertexNormals[vn-1];
        //}

        return new Vertex(vector);
    }

    public void ParseVertexLine(string line)
    {
        var split = line.Split(' ');

        if (split.Length is < 4 or > 5)
            return;
        
        // TODO: Figure out what to do with w
        float w = 1f;
        var x = float.Parse(split[1]);
        var y = float.Parse(split[2]);
        var z = float.Parse(split[3]);
        if (split.Length is 5)
            w = float.Parse(split[4]);

        var vector = new Vector3(x, y, z);
        _vertices.Add(vector);
    }

    private void FinishObject()
    {
        if (_currentMesh.Geometry.Faces.Count != 0)
            _meshes.Add(_currentMesh);
    }
}
