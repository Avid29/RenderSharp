using RenderSharp.Common.Materials;
using System.Collections.Generic;

namespace RenderSharp.Common.Objects.Meshes
{
    public class Mesh : IObject
    {
        public List<Face> _faces;

        public IMaterial Material { get; }
    }
}
