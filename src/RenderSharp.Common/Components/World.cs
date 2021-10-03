using RenderSharp.Common.Objects;
using RenderSharp.Common.Skys;
using System.Collections.Generic;

namespace RenderSharp.Common.Components
{
    public class World
    {
        public World(Sky sky)
        {
            Sky = sky;
            Spheres = new List<Sphere>();
        }

        public Sky Sky { get; }

        public List<Sphere> Spheres { get; }
    }
}
