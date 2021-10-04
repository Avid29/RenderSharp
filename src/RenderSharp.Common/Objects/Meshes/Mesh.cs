using RenderSharp.Common.Materials;
using System.Collections.Generic;
using System.Numerics;

namespace RenderSharp.Common.Objects.Meshes
{
    public class Mesh : IObject
    {
        public Mesh()
        {
            Verticies = new List<Vector3>();
            Faces = new List<Face>();
        }

        public List<Vector3> Verticies { get; }

        public List<Face> Faces { get; }

        public IMaterial Material { get; set; }
    }
}
