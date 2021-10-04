using ComputeSharp;
using RenderSharp.Common.Materials;
using RenderSharp.Common.Objects.Meshes;
using System.Collections.Generic;
using System.Numerics;
using CommonCamera = RenderSharp.Common.Components.Camera;
using CommonObject = RenderSharp.Common.Objects.IObject;
using CommonScene = RenderSharp.Common.Components.Scene;
using CommonSky = RenderSharp.Common.Skys.Sky;
using CommonSphere = RenderSharp.Common.Objects.Sphere;
using CommonWorld = RenderSharp.Common.Components.World;
using ShaderCamera = RenderSharp.RayTracing.HLSL.Components.Camera;
using ShaderMaterial = RenderSharp.RayTracing.HLSL.Materials.Material;
using ShaderScene = RenderSharp.RayTracing.HLSL.Components.Scene;
using ShaderSky = RenderSharp.RayTracing.HLSL.Skys.Sky;
using ShaderSphere = RenderSharp.RayTracing.HLSL.Geometry.Sphere;
using ShaderTriangle = RenderSharp.RayTracing.HLSL.Geometry.Triangle;
using ShaderWorld = RenderSharp.RayTracing.HLSL.Components.World;

namespace RenderSharp.RayTracing.HLSL.Conversion
{
    public class SceneConverter
    {
        private List<ShaderTriangle> _geometries;
        private GraphicsDevice _gpu;
        private Dictionary<IMaterial, int> _materialMap = new Dictionary<IMaterial, int>();
        private ReadOnlyBuffer<ShaderTriangle> _geometryBuffer;
        private ReadOnlyBuffer<ShaderMaterial> _materialBuffer;
        private bool _isGeometryLoaded;
        private bool _areMaterialsLoaded;

        public SceneConverter(GraphicsDevice gpu)
        {
            _gpu = gpu;
            _isGeometryLoaded = false;
            _geometries = new List<ShaderTriangle>();
        }

        public bool IsGeomertyLoaded => _isGeometryLoaded;

        public bool AreMaterialsLoaded => _areMaterialsLoaded;

        public ReadOnlyBuffer<ShaderTriangle> GeometryBuffer => _geometryBuffer;

        public ReadOnlyBuffer<ShaderMaterial> MaterialBuffer => _materialBuffer;

        public ShaderScene ConvertScene(CommonScene scene)
        {
            ShaderScene output;
            output.camera = ConvertCamera(scene.Camera);
            output.world = ConvertWorld(scene.World);

            // Default config for now
            output.config.samples = 64;
            output.config.maxBounces = 64;

            FinishMaterialLoading();

            return output;
        }

        public ShaderWorld ConvertWorld(CommonWorld world)
        {
            ShaderWorld output;
            output.sky = ConvertSky(world.Sky);

            for (int i = 0; i < world.Geometry.Count; i++)
            {
                ConvertObject(world.Geometry[i]);
            }

            LoadGeometry(_geometries.ToArray());

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
            return ShaderCamera.CreateCamera(camera.Origin, camera.Look, camera.FocalLength, camera.FOV, camera.Aperture);
        }

        public void ConvertObject(CommonObject @object)
        {
            int matId;

            if (_materialMap.ContainsKey(@object.Material))
            {
                matId = _materialMap[@object.Material];
            }
            else
            {
                int id = _materialMap.Count;
                _materialMap.Add(@object.Material, id);
                matId = id;
            }

            switch (@object)
            {
                case CommonSphere sphere:
                    // Shader does not display spheres
                    break;
                case Mesh mesh:
                    ConvertMesh(mesh, matId);
                    break;
            }
        }

        public void ConvertMesh(Mesh mesh, int matId)
        {
            // TODO: Triangluate faces
            // Uses only first 3 verticies of a face for now
            foreach (var face in mesh.Faces)
            {
                Vector3 a = face.Verticies[0];
                Vector3 b = face.Verticies[1];
                Vector3 c = face.Verticies[2];
                _geometries.Add(ShaderTriangle.Create(a, b, c, matId));
            }
        }

        public ShaderMaterial ConvertMaterial(IMaterial material)
        {
            // Create default material
            ShaderMaterial output;
            output.albedo = Float4.Zero;
            output.emission = Float4.Zero;
            output.metallic = 0;
            output.roughness = 0;

            switch (material)
            {
                case DiffuseMaterial diffuse:
                    output.albedo = diffuse.Albedo;
                    output.roughness = diffuse.Roughness;
                    break;
                case EmissiveMaterial emissive:
                    output.emission = (Vector4)emissive.Emission * emissive.Strength;
                    output.albedo = emissive.Emission;
                    output.roughness = 1;
                    break;
                case MetalMaterial metal:
                    output.albedo = metal.Albedo;
                    output.roughness = metal.Roughness;
                    break;
            }

            return output;
        }

        public void LoadGeometry(ShaderTriangle[] geometry)
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
