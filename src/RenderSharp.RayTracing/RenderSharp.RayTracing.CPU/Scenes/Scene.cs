using RenderSharp.RayTracing.CPU.Scenes.Cameras;

namespace RenderSharp.RayTracing.CPU.Scenes
{
    public struct Scene
    {
        public Scene(Camera camera, World world, RayTracingConfig config)
        {
            Camera = camera;
            World = world;
            Config = config;
        }

        public Camera Camera { get; }

        public World World { get; }

        public RayTracingConfig Config { get; }
    }
}
