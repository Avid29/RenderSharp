using ComputeSharp;
using CommonCamera = RenderSharp.Common.Components.Camera;
using CommonScene = RenderSharp.Common.Components.Scene;
using CommonSky = RenderSharp.Common.Skys.Sky;
using CommonSphere = RenderSharp.Common.Objects.Sphere;
using CommonWorld = RenderSharp.Common.Components.World;
using ShaderCamera = RenderSharp.RayTracing.HLSL.Components.Camera;
using ShaderScene = RenderSharp.RayTracing.HLSL.Components.Scene;
using ShaderSky = RenderSharp.RayTracing.HLSL.Skys.Sky;
using ShaderSphere = RenderSharp.RayTracing.HLSL.Geometry.Sphere;
using ShaderWorld = RenderSharp.RayTracing.HLSL.Components.World;

namespace RenderSharp.RayTracing.HLSL.Conversion
{
    public class SceneConverter
    {
        private GraphicsDevice _gpu;
        private ReadOnlyBuffer<ShaderSphere> _geometryBuffer;
        private bool _isGeometryLoaded;

        public SceneConverter(GraphicsDevice gpu)
        {
            _gpu = gpu;
            _isGeometryLoaded = false;
        }

        public bool IsGeomertyLoaded => _isGeometryLoaded;

        public ReadOnlyBuffer<ShaderSphere> GeometryBuffer => _geometryBuffer;

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

            ShaderSphere[] spheres = new ShaderSphere[world.Spheres.Count];

            for (int i = 0; i < world.Spheres.Count; i++)
            {
                spheres[i] = ConvertSphere(world.Spheres[i]);
            }

            LoadGeometry(spheres);

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

        public void LoadGeometry(ShaderSphere[] geometry)
        {
            if (_isGeometryLoaded)
                _geometryBuffer.Dispose();

            _isGeometryLoaded = true;

            _geometryBuffer = _gpu.AllocateReadOnlyBuffer(geometry);
        }

        public ShaderSphere ConvertSphere(CommonSphere sphere)
        {
            ShaderSphere output;
            output.center = sphere.Center;
            output.radius = sphere.Radius;
            return output;
        }
    }
}
