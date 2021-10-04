using RenderSharp.Common.Materials;
using RenderSharp.Common.Objects.Meshes;
using RenderSharp.RayTracing.CPU.Components;
using RenderSharp.RayTracing.CPU.Geometry;
using RenderSharp.RayTracing.CPU.Materials;
using System.Collections.Generic;
using System.Numerics;
using CommonCamera = RenderSharp.Common.Components.Camera;
using CommonDiffuse = RenderSharp.Common.Materials.DiffuseMaterial;
using CommonMaterial = RenderSharp.Common.Materials.IMaterial;
using CommonObject = RenderSharp.Common.Objects.IObject;
using CommonScene = RenderSharp.Common.Components.Scene;
using CommonSky = RenderSharp.Common.Skys.Sky;
using CommonSphere = RenderSharp.Common.Objects.Sphere;
using CommonWorld = RenderSharp.Common.Components.World;
using ShaderCamera = RenderSharp.RayTracing.CPU.Components.Camera;
using ShaderDiffuse = RenderSharp.RayTracing.CPU.Materials.DiffuseMaterial;
using ShaderGeometry = RenderSharp.RayTracing.CPU.Geometry.IGeometry;
using ShaderMaterial = RenderSharp.RayTracing.CPU.Materials.IMaterial;
using ShaderScene = RenderSharp.RayTracing.CPU.Components.Scene;
using ShaderSky = RenderSharp.RayTracing.CPU.Skys.Sky;
using ShaderSphere = RenderSharp.RayTracing.CPU.Geometry.Sphere;
using ShaderWorld = RenderSharp.RayTracing.CPU.Components.World;

namespace RenderSharp.RayTracing.CPU.Conversion
{
    public class SceneConverter
    {
        private List<ShaderGeometry> _geometries;

        public SceneConverter()
        {
            _geometries = new List<ShaderGeometry>();
        }

        public ShaderScene ConvertScene(CommonScene scene)
        {
            ShaderCamera camera = ConvertCamera(scene.Camera);
            ShaderWorld world = ConvertWorld(scene.World);

            // Default config for now
            RayTracingConfig config = new RayTracingConfig(16, 12);

            return new ShaderScene(camera, world, config);
        }

        public ShaderWorld ConvertWorld(CommonWorld world)
        {
            ShaderSky sky = ConvertSky(world.Sky);

            for (int i = 0; i < world.Geometry.Count; i++)
            {
                ConvertGeometry(world.Geometry[i]);
            }

            return new ShaderWorld(sky, _geometries.ToArray());
        }

        public ShaderSky ConvertSky(CommonSky sky)
        {
            return new ShaderSky(sky.Color);
        }

        public ShaderCamera ConvertCamera(CommonCamera camera)
        {
            return new ShaderCamera(camera.Origin, camera.Look, camera.FocalLength, camera.FOV, camera.Aperture);
        }

        public void ConvertGeometry(CommonObject @object)
        {
            ShaderMaterial material = ConvertMaterial(@object.Material);
            switch (@object)
            {
                case CommonSphere sphere:
                    _geometries.Add(new ShaderSphere(sphere.Center, sphere.Radius, material));
                    break;
                case Mesh mesh:
                    ConvertMesh(mesh);
                    break;
            }
        }

        public void ConvertMesh(Mesh mesh)
        {
            ShaderMaterial material = ConvertMaterial(mesh.Material);

            // TODO: Triangluate faces
            // Uses only first 3 verticies of a face for now
            foreach (var face in mesh.Faces)
            {
                Vector3 a = face.Verticies[0];
                Vector3 b = face.Verticies[1];
                Vector3 c = face.Verticies[2];
                _geometries.Add(new Triangle(a, b, c, material));
            }
        }

        public ShaderMaterial ConvertMaterial(CommonMaterial material)
        {
            switch (material)
            {
                case CommonDiffuse diffuse:
                    return new ShaderDiffuse(diffuse.Albedo, diffuse.Roughness);
                case MetalMaterial metal:
                    return new SuperMaterial(metal.Albedo, 1f, metal.Roughness, Vector4.Zero);
                case EmissiveMaterial emission:
                    return new SuperMaterial(emission.Emission, 0f, 1f, emission.Emission);
                default:
                    return null;
            }
        }
    }
}
