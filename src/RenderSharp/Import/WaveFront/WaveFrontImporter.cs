using RenderSharp.Common.Scenes.Materials;
using RenderSharp.Common.Scenes.Objects.Meshes;
using System.Collections.Generic;
using System.IO;

namespace RenderSharp.Import.WaveFront
{
    public class WaveFrontImporter
    {
        public List<Mesh> Objects { get; private set; }

        public Dictionary<string, IMaterial> Materials { get; private set; }

        public WaveFrontImporter(string objpath, string mtlPath = null)
        {
            if (mtlPath == null)
            {
                int preExtension = objpath.Length - 3;
                mtlPath = objpath.Substring(0, preExtension) + "mtl";
            }

            if (File.Exists(mtlPath))
            {
                ParseMaterials(mtlPath);
            }

            ParseObjects(objpath);
        }

        public void ParseObjects(string objPath)
        {
            Objects = new List<Mesh>();
            WaveFrontObj parser = new(objPath, Objects, Materials);
            parser.Parse();
        }

        public void ParseMaterials(string mtlPath)
        {
            Materials = new Dictionary<string, IMaterial>();
            WaveFrontMtl parser = new(mtlPath, Materials);
            parser.Parse();
        }
    }
}
