using ComputeSharp;
using RenderSharp.Common.Scenes.Materials;
using RenderSharp.Common.Scenes.Objects.Meshes;
using RenderSharp.RayTracing.HLSL.Scenes.BVH;
using System;
using System.Collections.Generic;
using System.Numerics;
using CommonCamera = RenderSharp.Common.Scenes.Camera;
using CommonObject = RenderSharp.Common.Scenes.Objects.IObject;
using CommonScene = RenderSharp.Common.Scenes.Scene;
using CommonSky = RenderSharp.Common.Scenes.Skys.Sky;
using CommonSphere = RenderSharp.Common.Scenes.Objects.Sphere;
using CommonWorld = RenderSharp.Common.Scenes.World;
using ShaderCamera = RenderSharp.RayTracing.HLSL.Scenes.Cameras.Camera;
using ShaderMaterial = RenderSharp.RayTracing.HLSL.Scenes.Materials.Material;
using ShaderScene = RenderSharp.RayTracing.HLSL.Scenes.Scene;
using ShaderSky = RenderSharp.RayTracing.HLSL.Scenes.Skys.Sky;
using ShaderTriangle = RenderSharp.RayTracing.HLSL.Scenes.Geometry.Triangle;
using ShaderWorld = RenderSharp.RayTracing.HLSL.Scenes.World;

namespace RenderSharp.RayTracing.HLSL.Conversion
{
    public class SceneConverter
    {
        private List<ShaderTriangle> _geometries;
        private BVHNode[] _bvhBuffer;
        private GraphicsDevice _gpu;
        private Dictionary<IMaterial, int> _materialMap = new Dictionary<IMaterial, int>();
        private ReadOnlyBuffer<ShaderTriangle> _geometryBuffer;
        private ReadOnlyBuffer<ShaderMaterial> _materialBuffer;
        private ReadOnlyBuffer<BVHNode> _bvhHeap;
        private int _bvhDepth;
        private int _bvhPos;
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

        public ReadOnlyBuffer<BVHNode> BVHHeap => _bvhHeap;

        public int BVHDepth => _bvhDepth;

        public ShaderScene ConvertScene(CommonScene scene)
        {
            ShaderScene output;
            output.camera = ConvertCamera(scene.Camera);
            output.world = ConvertWorld(scene.World);

            // Default config for now
            output.config.samples = 64;
            output.config.maxBounces = 12;

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

            BuildBVHTree(_geometries.ToArray());

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

        public void BuildBVHTree(ShaderTriangle[] geometries)
        {
            _bvhDepth = 0;
            _bvhBuffer = new BVHNode[(geometries.Length * 2) - 1];
            _bvhPos = _bvhBuffer.Length - 1;
            BuildBVH(geometries, 0, 0);
            _bvhHeap = _gpu.AllocateReadOnlyBuffer(_bvhBuffer);
            _geometryBuffer = _gpu.AllocateReadOnlyBuffer(geometries);
        }

        public int BuildBVH(Span<ShaderTriangle> geometries, int index, int depth)
        {
            int axis = depth;

            if (_bvhDepth < depth) _bvhDepth = depth;

            BVHNode node;
            node.geoI = -1;
            node.leftI = -1;
            node.rightI = -1;

            if (geometries.Length == 1)
            {
                node.geoI = index;
                node.boundingBox = ShaderTriangle.GetBoundingBox(geometries[0]);
            } else
            {
                geometries.Sort((a, b) => ShaderTriangle.GetBoundingBox(a).maximum[axis].CompareTo(ShaderTriangle.GetBoundingBox(b).maximum[axis]));

                int mid = geometries.Length / 2;
                node.rightI = BuildBVH(geometries.Slice(mid, geometries.Length - mid), index + mid, depth + 1);
                node.leftI = BuildBVH(geometries.Slice(0, mid), index, depth + 1);

                AABB rightBB = _bvhBuffer[node.rightI].boundingBox;
                AABB leftBB = _bvhBuffer[node.leftI].boundingBox;

                node.boundingBox = AABB.GetSurroundingBox(rightBB, leftBB);
            }

            _bvhBuffer[_bvhPos] = node;
            return _bvhPos--;
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
