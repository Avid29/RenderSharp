using RenderSharp.Common.Scenes.Objects;
using RenderSharp.Common.Scenes.Skys;
using System.Collections.Generic;

namespace RenderSharp.Common.Scenes
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
