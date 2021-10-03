using ComputeSharp;
using RenderSharp.Common.Skys;

namespace RenderSharp.Common.Components
{
    public class Scene
    {
        public Scene(Camera camera, World world)
        {
            Camera = camera;
            World = world;
        }

        public Camera Camera { get; }

        public World World { get; }

        public static Scene CreateEmptyScene()
        {
            Camera camera = new Camera(Float3.UnitZ, 2f, 1f);
            Sky sky = new Sky(new Float4(0.5f, 0.7f, 1f, 1f));
            World world = new World(sky);
            return new Scene(camera, world);
        }
    }
}
