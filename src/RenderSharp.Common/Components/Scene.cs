using ComputeSharp;
using RenderSharp.Common.Materials;
using RenderSharp.Common.Objects;
using RenderSharp.Common.Skys;
using RenderSharp.Common.Textures;

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
            Camera camera = new Camera(new Float3(0f, 1f, 2f), new Float3(0f, 0f, 0f), 90f, 0.2f);
            Sky sky = new Sky(new Float4(0.5f, 0.7f, 1f, 1f));
            World world = new World(sky);
            return new Scene(camera, world);
        }

        public static Scene CreateSpheresScene()
        {
            Float4 white = Float4.One;
            Float4 darkGreen = new Float4(0.2f, 0.3f, 0.1f, 1f);

            CheckersTexture texture = new CheckersTexture(white, darkGreen);

            DiffuseMaterial diffuse1 = new DiffuseMaterial(new SolidTexture(new Float4(1f, 0f, 0f, 1f)), 0.5f);
            DiffuseMaterial diffuse2 = new DiffuseMaterial(new SolidTexture(new Float4(0.4f, 0.5f, 0.5f, 1f)), 0.8f);
            DiffuseMaterial diffuse3 = new DiffuseMaterial(texture, 0.5f);
            MetalMaterial rawMetal = new MetalMaterial(new SolidTexture(new Float4(0.8f, 0.8f, 0.8f, 1f)), 0f);
            EmissiveMaterial emissive = new EmissiveMaterial(Float4.One, 2f);

            Scene scene = CreateEmptyScene();
            scene.World.Spheres.Add(new Sphere(Float3.Zero, 0.5f, diffuse3));
            scene.World.Spheres.Add(new Sphere(new Float3(0f, 0f, -1f), 0.5f, diffuse1));
            scene.World.Spheres.Add(new Sphere(new Float3(1f, 0f, 0f), 0.5f, rawMetal));
            scene.World.Spheres.Add(new Sphere(new Float3(-1f, 0f, 0f), 0.5f, emissive));
            scene.World.Spheres.Add(new Sphere(new Float3(0, -100.5f, 0), 100f, diffuse2));
            return scene;
        }
    }
}
