using RenderSharp.Common.Objects.Meshes;
using System;
using System.IO;
using System.Numerics;

namespace RenderSharp.Import
{
    public class WaveFrontImporter
    {
        public static Mesh LoadMesh(string filePath)
        {
            Mesh mesh = new Mesh();
            using (StreamReader stream = File.OpenText(filePath))
            {
                while (!stream.EndOfStream)
                {
                    string line = stream.ReadLine();
                    ParseStreamLine(line, mesh);
                }
            }

            return mesh;
        }

        private static void ParseStreamLine(string line, Mesh mesh)
        {
            string[] parts = line.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 0)
            {
                switch (parts[0])
                {
                    case "v":
                        Vector3 v = ParseVertex(parts);
                        mesh.Verticies.Add(v);
                        break;
                    case "f":
                        Face f = ParseFace(parts, mesh);
                        mesh.Faces.Add(f);
                        break;
                }
            }
        }

        private static Vector3 ParseVertex(string[] parts)
        {
            float x, y, z;
            float.TryParse(parts[1], out x);
            float.TryParse(parts[2], out y);
            float.TryParse(parts[3], out z);
            return new Vector3(x, y, z);
        }

        private static Face ParseFace(string[] parts, Mesh mesh)
        {
            // TODO: Parse texture coordinates

            Face face = new Face();
            for (int i = 1; i < parts.Length; i++)
            {
                int vIndex;
                int.TryParse(parts[i], out vIndex);
                face.Verticies.Add(mesh.Verticies[vIndex-1]);
            }

            return face;
        }
    }
}
