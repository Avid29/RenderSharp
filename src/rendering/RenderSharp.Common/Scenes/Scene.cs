using RenderSharp.Scenes.Materials;
using RenderSharp.Scenes.Objects;
using RenderSharp.Scenes.Objects.Meshes;
using RenderSharp.Scenes.Skys;
using System.Numerics;

namespace RenderSharp.Scenes
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
            Camera camera = new Camera(new Vector3(0f, 1f, 2f), new Vector3(0f, 0f, 0f), 90f, 0.2f);
            Sky sky = new Sky(new Vector4(0.5f, 0.7f, 1f, 1f));
            World world = new World(sky);
            return new Scene(camera, world);
        }

        public static Scene CreateSpheresScene()
        {
            DiffuseMaterial diffuse1 = new DiffuseMaterial(new Vector4(1f, 0f, 0f, 1f), 0.5f);
            DiffuseMaterial diffuse2 = new DiffuseMaterial(new Vector4(0.4f, 0.5f, 0.5f, 0.8f));
            GlossyMaterial rawMetal = new GlossyMaterial(new Vector4(0.8f, 0.8f, 0.8f, 1f), 0f);
            EmissiveMaterial emissive = new EmissiveMaterial(Vector4.One, 2f);

            Scene scene = CreateEmptyScene();
            scene.World.Geometry.Add(new Sphere(Vector3.Zero, 0.5f, diffuse1));
            scene.World.Geometry.Add(new Sphere(new Vector3(1f, 0f, 0f), 0.5f, rawMetal));
            scene.World.Geometry.Add(new Sphere(new Vector3(-1f, 0f, 0f), 0.5f, emissive));
            scene.World.Geometry.Add(new Sphere(new Vector3(0, -100.5f, 0), 100f, diffuse2));
            return scene;
        }

        public static Scene CreateMeshScene(Mesh mesh)
        {
            DiffuseMaterial diffuse = new DiffuseMaterial(new Vector4(0.5f, 0.5f, 0.5f, 1f), 0.5f);
            mesh.Material = diffuse;

            Scene scene = CreateEmptyScene();
            scene.Camera = new Camera(new Vector3(1.75f, 1.5f, 2f), new Vector3(0f, 0f, 0f), 90f, 0.01f);
            scene.World.Geometry.Add(mesh);
            return scene;
        }
    }
}
