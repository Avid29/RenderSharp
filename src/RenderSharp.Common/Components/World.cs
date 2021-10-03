using RenderSharp.Common.Skys;

namespace RenderSharp.Common.Components
{
    public class World
    {
        public World(Sky sky)
        {
            Sky = sky;
        }

        public Sky Sky { get; }

        //public List<Sphere> Spheres { get; }
    }
}
