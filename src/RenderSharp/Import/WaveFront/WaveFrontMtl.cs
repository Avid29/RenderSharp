using RenderSharp.Common.Scenes.Materials;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace RenderSharp.Import.WaveFront
{
    public class WaveFrontMtl
    {
        string _filePath;
        Dictionary<string, IMaterial> _materials;
        SuperMaterial _activeMaterial;

        public WaveFrontMtl(string filePath, Dictionary<string, IMaterial> materials)
        {
            _filePath = filePath;
            _materials = materials;
        }

        public void Parse()
        {
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
                    case "newmtl":
                        _activeMaterial = new SuperMaterial();
                        _activeMaterial.Name = parts[1];
                        _materials.Add(_activeMaterial.Name, _activeMaterial);
                        break;
                    case "Kd":
                        _activeMaterial.Albedo = ParseColor(parts);
                        break;
                    case "Ke":
                        _activeMaterial.Emission = ParseColor(parts);
                        break;
                }
            }
        }

        private Vector4 ParseColor(string[] parts)
        {
            if (parts.Length < 4) return Vector4.Zero;

            float a = 1;
            float r = float.Parse(parts[1]);
            float g = float.Parse(parts[2]);
            float b = float.Parse(parts[3]);
            
            if (parts.Length > 4)
            a = float.Parse(parts[4]);

            return new Vector4(r, g, b, a);
        }
    }
}
