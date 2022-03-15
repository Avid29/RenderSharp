using RenderSharp.RayTracing.Scenes.Cameras;

namespace RenderSharp.RayTracing.Scenes
{
    public struct Scene
    {
        public Camera camera;
        public Config config;
        public World world;
    }
}
