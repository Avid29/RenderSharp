using ComputeSharp;
using RenderSharp.Common.Materials;
using System.Collections.Generic;

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
using ShaderMaterial = RenderSharp.RayTracing.HLSL.Materials.Material;

namespace RenderSharp.RayTracing.HLSL.Conversion
{
    public class SceneConverter
    {
        private GraphicsDevice _gpu;
        private Dictionary<DiffuseMaterial, int> _materialMap = new Dictionary<DiffuseMaterial, int>();
        private ReadOnlyBuffer<ShaderSphere> _geometryBuffer;
        private ReadOnlyBuffer<ShaderMaterial> _materialBuffer;
        private bool _isGeometryLoaded;
        private bool _areMaterialsLoaded;

        public SceneConverter(GraphicsDevice gpu)
        {
            _gpu = gpu;
            _isGeometryLoaded = false;
        }

        public bool IsGeomertyLoaded => _isGeometryLoaded;

        public bool AreMaterialsLoaded => _areMaterialsLoaded;

        public ReadOnlyBuffer<ShaderSphere> GeometryBuffer => _geometryBuffer;

        public ReadOnlyBuffer<ShaderMaterial> MaterialBuffer => _materialBuffer;

        public ShaderScene ConvertScene(CommonScene scene)
        {
            ShaderScene output;
            output.camera = ConvertCamera(scene.Camera);
            output.world = ConvertWorld(scene.World);

            // Default config for now
            output.config.samples = 16;
            output.config.maxBounces = 16;

            FinishMaterialLoading();

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

        public ShaderSphere ConvertSphere(CommonSphere sphere)
        {
            ShaderSphere output;
            output.center = sphere.Center;
            output.radius = sphere.Radius;
            
            if (_materialMap.ContainsKey(sphere.Material))
            {
                output.matId = _materialMap[sphere.Material];
            } else
            {
                int id = _materialMap.Count;
                _materialMap.Add(sphere.Material, id);
                output.matId = id;
            }

            return output;
        }

        public ShaderMaterial ConvertMaterial(DiffuseMaterial material)
        {
            ShaderMaterial output;
            output.albedo = material.Albedo;
            return output;
        }

        public void LoadGeometry(ShaderSphere[] geometry)
        {
            if (_isGeometryLoaded)
                _geometryBuffer.Dispose();

            _geometryBuffer = _gpu.AllocateReadOnlyBuffer(geometry);
            _isGeometryLoaded = true;
        }

        public void FinishMaterialLoading()
        {
            if (_areMaterialsLoaded)
                _materialBuffer.Dispose();

            ShaderMaterial[] materials = new ShaderMaterial[_materialMap.Count];

            int i = 0;
            foreach (var material in _materialMap.Keys)
            {
                materials[i] = ConvertMaterial(material);
                i++;
            }

            _materialBuffer = _gpu.AllocateReadOnlyBuffer(materials);
            _areMaterialsLoaded = true;
        }
    }
}
