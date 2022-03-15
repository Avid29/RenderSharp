using System.Collections.Generic;
using System.Numerics;

namespace RenderSharp.Scenes.Objects.Meshes
{
    public class Face
    {
        public Face()
        {
            Verticies = new List<Vector3>();
        }

        public List<Vector3> Verticies { get; }
    }
}
