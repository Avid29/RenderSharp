using ComputeSharp;

using CommonCamera = RenderSharp.Common.Components.Camera;
using CommonScene = RenderSharp.Common.Components.Scene;
using CommonSky = RenderSharp.Common.Skys.Sky;
using CommonWorld = RenderSharp.Common.Components.World;
using ShaderCamera = RenderSharp.RayTracing.HLSL.Components.Camera;
using ShaderScene = RenderSharp.RayTracing.HLSL.Components.Scene;
using ShaderSky = RenderSharp.RayTracing.HLSL.Skys.Sky;
using ShaderWorld = RenderSharp.RayTracing.HLSL.Components.World;

namespace RenderSharp.RayTracing.HLSL.Conversion
{
    public class SceneConverter
    {
        private GraphicsDevice _gpu;

        public SceneConverter(GraphicsDevice gpu)
        {
            _gpu = gpu;
        }

        public ShaderScene ConvertScene(CommonScene scene)
        {
            ShaderScene output;
            output.camera = ConvertCamera(scene.Camera);
            output.world = ConvertWorld(scene.World);

            // Default config for now
            output.config.samples = 16;
            output.config.maxBounces = 16;

            return output;
        }

        public ShaderWorld ConvertWorld(CommonWorld world)
        {
            ShaderWorld output;
            output.sky = ConvertSky(world.Sky);
            return output;
        }

        public ShaderSky ConvertSky(CommonSky sky)
        {
            ShaderSky output;
            output.color = sky.Color;
            return output;
        }

        public ShaderCamera ConvertCamera(CommonCamera camera)
        {
            return ShaderCamera.CreateCamera(camera.Origin, camera.FocalLength, camera.FOV);
        }
    }
}
