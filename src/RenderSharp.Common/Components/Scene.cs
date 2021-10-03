using ComputeSharp;
using RenderSharp.Common.Objects;
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

        public static Scene CreateSpheresScene()
        {
            Scene scene = CreateEmptyScene();
            scene.World.Spheres.Add(new Sphere(Float3.Zero, 0.5f));
            scene.World.Spheres.Add(new Sphere(new Float3(0, -100.5f, 0), 100f));
            return scene;
        }
    }
}
