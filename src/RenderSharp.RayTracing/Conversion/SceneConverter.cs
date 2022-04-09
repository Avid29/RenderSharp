using RenderSharp.RayTracing.Scenes.BVH;
using RenderSharp.Scenes.Objects;
using RenderSharp.Scenes.Objects.Meshes;
using System;
using System.Collections.Generic;
using System.Numerics;
using Common = RenderSharp.Scenes;
using RayTrace = RenderSharp.RayTracing.Scenes;

namespace RenderSharp.RayTracing.Conversion
{
    public class SceneConverter
    {
        private List<RayTrace.Geometry.Triangle> _geometries;
        private BVHNode[] _bvhHeap;
        private RayTrace.Geometry.Triangle[] _geometryBuffer;
        private int _bvhDepth;
        private int _bvhPos;

        public SceneConverter()
        {
            _geometries = new List<RayTrace.Geometry.Triangle>();
        }

        public BVHNode[] BVHHeap => _bvhHeap;

        public RayTrace.Geometry.Triangle[] GeometryBuffer => _geometryBuffer;

        public int BVHDepth => _bvhDepth;

        public RayTrace.Scene ConvertScene(Common.Scene scene)
        {
            RayTrace.Scene output;
            output.camera = ConvertCamera(scene.Camera);
            output.world = ConvertWorld(scene.World);

            output.config.samples = 32;
            output.config.maxBounces = 12;

            return output;
        }

        private RayTrace.Cameras.Camera ConvertCamera(Common.Camera camera)
        {
            return RayTrace.Cameras.Camera.Create(camera.Origin, camera.Look, camera.FocalLength, camera.FOV, camera.Aperture);
        }

        private RayTrace.World ConvertWorld(Common.World world)
        {
            RayTrace.World output;
            output.sky = ConvertSky(world.Sky);
            ConvertObjects(world.Geometry);
            return output;
        }

        private RayTrace.Skys.Sky ConvertSky(Common.Skys.Sky sky)
        {
            RayTrace.Skys.Sky output;
            output.albedo = sky.Albedo;
            return output;
        }

        private void ConvertObjects(List<IObject> objects)
        {
            foreach (var @object in objects)
            {
                ConvertObject(@object);
            }

            BuildBVHTree();
        }

        private void ConvertObject(IObject @object)
        {
            // TODO: Materials

            switch (@object)
            {
                case Mesh mesh:
                    ConvertMesh(mesh);
                    break;
            }
        }

        private void ConvertMesh(Mesh mesh)
        {
            // TODO: Triangluate faces
            foreach (var face in mesh.Faces)
            {
                Vector3 a = face.Verticies[0];
                Vector3 b = face.Verticies[1];
                Vector3 c = face.Verticies[2];
                _geometries.Add(RayTrace.Geometry.Triangle.Create(a, b, c, 0)); // TODO: Material Id
            }
        }

        private void BuildBVHTree()
        {
            _bvhDepth = 0;
            _geometryBuffer = _geometries.ToArray();
            _bvhHeap = new BVHNode[(_geometryBuffer.Length * 2) - 1];
            _bvhPos = _bvhHeap.Length - 1;
            BuildBVH(_geometryBuffer, 0, 0);
        }

        private int BuildBVH(Span<RayTrace.Geometry.Triangle> geometries, int index, int depth)
        {
            int axis = depth;

            if (_bvhDepth < depth) _bvhDepth = depth;

            BVHNode node;

            if (geometries.Length == 1)
            {
                AABB bounding = RayTrace.Geometry.Triangle.GetBoundingBox(geometries[0]);
                node = BVHNode.Create(bounding, index);
            } else
            {
#if NET5_0_OR_GREATER
                geometries.Sort((a, b) => ((float3)RayTrace.Geometry.Triangle.GetBoundingBox(a).maximum)[axis].CompareTo(((float3)RayTrace.Geometry.Triangle.GetBoundingBox(b).maximum)[axis]));
#endif
                int mid = geometries.Length / 2;
                int rightI = BuildBVH(geometries.Slice(mid, geometries.Length - mid), index + mid, depth + 1);
                int leftI = BuildBVH(geometries.Slice(0, mid), index, depth + 1);

                AABB rightBB = _bvhHeap[rightI].boundingBox;
                AABB leftBB = _bvhHeap[leftI].boundingBox;

                AABB bounding = AABB.GetSurroundingBox(rightBB, leftBB);
                node = BVHNode.Create(bounding, leftI, rightI);
            }

            _bvhHeap[_bvhPos] = node;
            return _bvhPos--;
        }
    }
}
