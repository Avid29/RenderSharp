using RenderSharp.Common.Scenes.Materials;
using RenderSharp.Common.Scenes.Objects.Meshes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace RenderSharp.Import.WaveFront
{
    public class WaveFrontObj
    {
        string _filePath;
        List<Vector3> _verticies;
        List<Mesh> _meshes;
        //Dictionary<string, Mesh> _meshDictionary;
        Mesh _activeMesh;

        Dictionary<string, IMaterial> _materialDictionary;
        IMaterial _activeMaterial;

        public WaveFrontObj(string filePath, List<Mesh> meshes, Dictionary<string, IMaterial> materials)
        {
            _meshes = meshes;
            _materialDictionary = materials;
            _filePath = filePath;
        }

        public void Parse()
        {
            _verticies = new List<Vector3>();
            using (StreamReader stream = File.OpenText(_filePath))
            {
                while (!stream.EndOfStream)
                {
                    string line = stream.ReadLine();
                    ParseStreamLine(line);
                }
            }
        }

        private void ParseStreamLine(string line)
        {
            string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 0)
            {
                switch (parts[0])
                {
                    case "o":
                        ParseObject(parts);
                        break;
                    case "v":
                        ParseVertex(parts);
                        break;
                    case "f":
                        ParseFace(parts);
                        break;
                    case "usemtl":
                        _activeMaterial = _materialDictionary[parts[1]];
                        _activeMesh.Material = _activeMaterial;
                        break;
                }
            }
        }

        private void ParseObject(string[] parts)
        {
            _activeMesh = new Mesh();
            _activeMesh.Name = parts[1];
            _meshes.Add(_activeMesh);
        }

        private void ParseVertex(string[] parts)
        {
            float x, y, z;
            float.TryParse(parts[1], out x);
            float.TryParse(parts[2], out y);
            float.TryParse(parts[3], out z);
            Vector3 vertex = new Vector3(x, y, z);
            _verticies.Add(vertex);
            _activeMesh.Verticies.Add(vertex);
        }

        private void ParseFace(string[] parts)
        {
            // TODO: Parse texture coordinates

            Face face = new Face();
            for (int i = 1; i < parts.Length; i++)
            {
                int vIndex;
                int.TryParse(parts[i], out vIndex);
                face.Verticies.Add(_verticies[vIndex - 1]);
            }

            _activeMesh.Faces.Add(face);
        }
    }
}
