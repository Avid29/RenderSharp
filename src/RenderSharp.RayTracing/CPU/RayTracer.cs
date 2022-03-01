using ComputeSharp;
using Microsoft.Toolkit.HighPerformance;
using RenderSharp.Buffer;
using RenderSharp.RayTracing.Conversion;
using RenderSharp.RayTracing.CPU.MockShaders;
using RenderSharp.RayTracing.CPU.MockShaders.Materials;
using RenderSharp.RayTracing.GPU.Shaders;
using RenderSharp.RayTracing.Scenes;
using RenderSharp.RayTracing.Scenes.BVH;
using RenderSharp.RayTracing.Scenes.Cameras;
using RenderSharp.RayTracing.Scenes.Geometry;
using RenderSharp.RayTracing.Scenes.Materials;
using RenderSharp.RayTracing.Scenes.Rays;
using RenderSharp.Render.Tiles;
using System;
using CommonScene = RenderSharp.Scenes.Scene;

namespace RenderSharp.RayTracing.CPU
{
    public class RayTracer
    {
        private Scene _scene;
        private FullCamera _camera;
        private int2 _fullSize;
        private CPUReadWriteImageBuffer _buffer;

        private Triangle[] _geometryBuffer;
        private BVHNode[] _bvhBuffer;
        private int _bvhDepth;
        
        public RayTracer(CommonScene scene, int2 fullsize, CPUReadWriteImageBuffer buffer)
        {
            SceneConverter converter = new SceneConverter();
            _scene = converter.ConvertScene(scene);
            _geometryBuffer = converter.GeometryBuffer;
            _bvhBuffer = converter.BVHHeap;

            _bvhDepth = converter.BVHDepth;
            _fullSize = fullsize;
            _buffer = buffer;

            _camera = FullCamera.Create(_scene.camera, (float)fullsize.X / fullsize.Y);
        }

        public void RenderTile(Tile tile)
        {
            // TODO: Multiple samples

            // Allocate buffers
            int[,,] bvhStack = new int[tile.Width, tile.Height, _bvhDepth + 1];
            Span<Ray> rayBuffer = new Ray[tile.Width * tile.Height];
            Span<RayCast> rayCastBuffer = new RayCast[tile.Width * tile.Height];
            Span2D<int> materialBuffer = new int[tile.Width, tile.Height];
            Span2D<float4> atteniationBuffer = new float4[tile.Width, tile.Height];
            Span2D<uint> randStates = new uint[tile.Width, tile.Height];

            new InitializeMockShader(_scene, tile.Offset, atteniationBuffer, randStates).Execute();
            new CameraCastMockShader(_scene, _camera, tile.Offset, _fullSize, rayBuffer, randStates).Execute(tile.Width, tile.Height);

            DiffuseMaterial diffuse = DiffuseMaterial.Create(float4.One * .8f, .5f);

            for (int i = 0; i < _scene.config.maxBounces; i++)
            {
                new CollisionMockShader(_scene, _geometryBuffer, _bvhBuffer, bvhStack, rayBuffer, rayCastBuffer, materialBuffer).Execute(tile.Width, tile.Height);
                new DiffuseMockShader(0, _scene, tile.Offset, tile.Size, diffuse, rayBuffer, rayCastBuffer, materialBuffer, atteniationBuffer, _buffer.AsSpan(), randStates).Execute(tile.Width, tile.Height);
            }
        }
    }
}
