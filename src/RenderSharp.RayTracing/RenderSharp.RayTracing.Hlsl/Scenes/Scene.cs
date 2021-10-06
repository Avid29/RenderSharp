using RenderSharp.RayTracing.HLSL.Scenes.Cameras;

namespace RenderSharp.RayTracing.HLSL.Scenes
{
    public struct Scene
    {
        public RayTracingConfig config;
        public Camera camera;
        public World world;
    }
}
