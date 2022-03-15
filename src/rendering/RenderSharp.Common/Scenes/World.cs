using RenderSharp.Scenes.Objects;
using RenderSharp.Scenes.Skys;
using System.Collections.Generic;

namespace RenderSharp.Scenes
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
