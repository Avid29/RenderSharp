using ComputeSharp;
using System.Collections.Generic;

namespace RenderSharp.Common.Objects.Meshes
{
    public class Face
    {
        public Face()
        {
            Verticies = new List<Float3>();
        }

        public List<Float3> Verticies { get; }
    }
}
