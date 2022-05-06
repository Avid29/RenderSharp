using RenderSharp.Scenes.Materials;
using System.Collections.Generic;

namespace RenderSharp.Scenes.Objects.Meshes
{
    public class Mesh : IObject
    {
        public Mesh() : this(string.Empty)
        {
        }

        public Mesh(string name)
        {
            Name = name;
            Faces = new List<Face>();
        }
        
        public string Name { get; }

        public List<Face> Faces { get; }

        public IMaterial Material { get; set; }
    }
}
