using RenderSharp.Scenes.Materials;
using RenderSharp.Scenes.Objects.Meshes;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace RenderSharp.Import
{
    public partial class WaveFrontImporter
    {
        private List<Vector3> _verticies;

        private List<Mesh> ParseObjects()
        {
            using (StreamReader stream = File.OpenText(_objFilePath))
            {
                Mesh activeMesh = new Mesh();
                MaterialBase activeMaterial = new DiffuseMaterial(new Vector4(Vector3.One * .5f, 1), .5f);
                while (!stream.EndOfStream)
                {
                    string line = stream.ReadLine();
                    ObjParseStreamLine(line, ref activeMesh, ref activeMaterial);
                }
            }

            return Objects;
        }

        private void ObjParseStreamLine(string line, ref Mesh activeMesh, ref MaterialBase activeMaterial)
        {
            string[] parts = line.Split(' ');
            if (parts.Length > 0)
            {
                switch (parts[0])
                {
                    case "#":
                        break;
                    case "o":
                        activeMesh = ParseObject(parts);
                        activeMesh.Material = activeMaterial;
                        Objects.Add(activeMesh);
                        break;
                    case "v":
                        Vector3 v = ParseVertex(parts);
                        _verticies.Add(v);
                        break;
                    case "f":
                        Face f = ParseFace(parts);
                        activeMesh.Faces.Add(f);
                        break;
                    case "usemtl":
                        activeMaterial = Materials[parts[1]];
                        activeMesh.Material = activeMaterial;
                        break;
                }
            }
        }

        private Mesh ParseObject(string[] parts)
        {
            return new Mesh(parts[1]);
        }

        private Vector3 ParseVertex(string[] parts)
        {
            float.TryParse(parts[1], out float x);
            float.TryParse(parts[2], out float y);
            float.TryParse(parts[3], out float z);
            var v3 = new Vector3(x, y, z);
            return v3;
        }

        private Face ParseFace(string[] parts)
        {
            // TODO: Parse texture coordinates

            Face face = new Face();
            for (int i = 1; i < parts.Length; i++)
            {
                int.TryParse(parts[i], out int vIndex);
                face.Verticies.Add(_verticies[vIndex - 1]);
            }

            return face;
        }
    }
}
