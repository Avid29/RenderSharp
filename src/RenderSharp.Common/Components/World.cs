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
            Geometry = new List<IObject>();
        }

        public Sky Sky { get; }

        public List<IObject> Geometry { get; }
    }
}
