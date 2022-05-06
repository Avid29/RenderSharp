using RenderSharp.Scenes.Materials;
using RenderSharp.Scenes.Objects.Meshes;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace RenderSharp.Import
{
    public partial class WaveFrontImporter
    {
        private string _objFilePath;
        private string _mtlFilePath;

        internal WaveFrontImporter(string objPath, string mtlPath = null, bool useMtl = true)
        {
            _objFilePath = objPath;

            if (useMtl)
            {
                mtlPath ??= objPath.Substring(0, objPath.Length - 3) + "mtl";

                if (File.Exists(mtlPath))
                {
                    _mtlFilePath = mtlPath;
                    Materials = new Dictionary<string, IMaterial>();
                }
            }

            _verticies = new List<Vector3>();
            Objects = new List<Mesh>();
        }

        private List<Mesh> Objects { get; }

        private Dictionary<string, IMaterial> Materials { get; }

        private void Parse()
        {
            ParseObjects();
        }

        public static List<Mesh> Parse(string objPath, string mtlPath = null, bool useMtl = true)
        {
            WaveFrontImporter importer = new WaveFrontImporter(objPath, mtlPath, useMtl);
            importer.Parse();
            return importer.Objects;
        }
    }
}
