using RenderSharp.Scenes.Materials;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace RenderSharp.Import
{
    public partial class WaveFrontImporter
    {
        private Dictionary<string, MaterialBase> ParseMaterials()
        {
            using (StreamReader stream = File.OpenText(_mtlFilePath))
            {
                SuperMaterial activeMaterial = new SuperMaterial();
                while (!stream.EndOfStream)
                {
                    string line = stream.ReadLine();
                    MtlParseStreamLine(line, ref activeMaterial);
                }
            }

            return Materials;
        }

        private void MtlParseStreamLine(string line, ref SuperMaterial activeMaterial)
        {
            string[] parts = line.Split(' ');
            if (parts.Length > 0)
            {
                switch (parts[0])
                {
                    case "newmtl":
                        activeMaterial = new SuperMaterial(parts[1]);
                        Materials.Add(activeMaterial.Name, activeMaterial);
                        break;
                    case "Kd":
                        activeMaterial.Albedo = ParseColor(parts);
                        break;
                    case "Ks":
                        activeMaterial.Specular = ParseFloat(parts);
                        break;
                    case "Ke":
                        activeMaterial.Emission = ParseColor(parts);
                        break;
                    case "Ni":
                        activeMaterial.Fresnel = ParseFloat(parts);
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

        private float ParseFloat(string[] parts)
        {
            return float.Parse(parts[1]);
        }
    }
}
