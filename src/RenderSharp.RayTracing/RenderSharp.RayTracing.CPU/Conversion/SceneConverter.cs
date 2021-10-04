﻿using RenderSharp.Common.Materials;
using RenderSharp.RayTracing.CPU.Components;
using RenderSharp.RayTracing.CPU.Materials;
using CommonCamera = RenderSharp.Common.Components.Camera;
using CommonDiffuse = RenderSharp.Common.Materials.DiffuseMaterial;
using CommonMaterial = RenderSharp.Common.Materials.IMaterial;
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
using System.Numerics;

namespace RenderSharp.RayTracing.CPU.Conversion
{
    public class SceneConverter
    {
        public SceneConverter()
        { }

        public ShaderScene ConvertScene(CommonScene scene)
        {
            ShaderCamera camera = ConvertCamera(scene.Camera);
            ShaderWorld world = ConvertWorld(scene.World);

            // Default config for now
            RayTracingConfig config = new RayTracingConfig(64, 12);

            return new ShaderScene(camera, world, config);
        }

        public ShaderWorld ConvertWorld(CommonWorld world)
        {
            ShaderSky sky = ConvertSky(world.Sky);

            ShaderGeometry[] geometries = new ShaderGeometry[world.Spheres.Count];

            for (int i = 0; i < world.Spheres.Count; i++)
            {
                geometries[i] = ConvertSphere(world.Spheres[i]);
            }

            return new ShaderWorld(sky, geometries);
        }

        public ShaderSky ConvertSky(CommonSky sky)
        {
            return new ShaderSky(sky.Color);
        }

        public ShaderCamera ConvertCamera(CommonCamera camera)
        {
            return new ShaderCamera(camera.Origin, camera.Look, camera.FocalLength, camera.FOV, camera.Aperture);
        }

        public ShaderGeometry ConvertSphere(CommonSphere sphere)
        {
            ShaderMaterial material = ConvertMaterial(sphere.Material);
            return new ShaderSphere(sphere.Center, sphere.Radius, material);
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