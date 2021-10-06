using ComputeSharp;
using RenderSharp.Common.Scenes.Materials;
using RenderSharp.Common.Scenes.Objects;
using RenderSharp.Common.Scenes.Objects.Meshes;
using RenderSharp.Common.Scenes.Skys;

namespace RenderSharp.Common.Scenes
{
    public class Scene
    {
        public Scene(Camera camera, World world)
        {
            Camera = camera;
            World = world;
        }

        public Camera Camera { get; set; }

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
            DiffuseMaterial diffuse1 = new DiffuseMaterial(new Float4(1f, 0f, 0f, 1f), 0.5f);
            DiffuseMaterial diffuse2 = new DiffuseMaterial(new Float4(0.4f, 0.5f, 0.5f, 0.8f));
            MetalMaterial rawMetal = new MetalMaterial(new Float4(0.8f, 0.8f, 0.8f, 1f), 0f);
            EmissiveMaterial emissive = new EmissiveMaterial(Float4.One, 2f);

            Scene scene = CreateEmptyScene();
            scene.World.Geometry.Add(new Sphere(Float3.Zero, 0.5f, diffuse1));
            scene.World.Geometry.Add(new Sphere(new Float3(1f, 0f, 0f), 0.5f, rawMetal));
            scene.World.Geometry.Add(new Sphere(new Float3(-1f, 0f, 0f), 0.5f, emissive));
            scene.World.Geometry.Add(new Sphere(new Float3(0, -100.5f, 0), 100f, diffuse2));
            return scene;
        }

        public static Scene CreateMeshScene(Mesh mesh)
        {
            DiffuseMaterial diffuse = new DiffuseMaterial(new Float4(0.5f, 0.5f, 0.5f, 1f), 0.5f);
            mesh.Material = diffuse;

            Scene scene = CreateEmptyScene();
            scene.Camera = new Camera(new Float3(1.75f, 1.5f, 2f), new Float3(0f, 0f, 0f), 90f, 0.01f);
            scene.World.Geometry.Add(mesh);
            return scene;
        }
    }
}
