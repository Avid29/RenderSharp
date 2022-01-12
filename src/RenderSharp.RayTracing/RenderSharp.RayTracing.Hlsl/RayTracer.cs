using ComputeSharp;
using RenderSharp.Common.Devices.Buffers;
using RenderSharp.Common.Render.Tiles;
using RenderSharp.RayTracing.HLSL.Conversion;
using RenderSharp.RayTracing.HLSL.Scenes;
using RenderSharp.RayTracing.HLSL.Scenes.BVH;
using RenderSharp.RayTracing.HLSL.Scenes.Cameras;
using RenderSharp.RayTracing.HLSL.Scenes.Geometry;
using RenderSharp.RayTracing.HLSL.Scenes.Materials;
using RenderSharp.RayTracing.HLSL.Scenes.Rays;
using RenderSharp.RayTracing.HLSL.Shaders;
using CommonScene = RenderSharp.Common.Scenes.Scene;

namespace RenderSharp.RayTracing.HLSL
{
    public class RayTracer
    {
        private Scene _scene;
        private FullCamera _camera;
        private int _bvhDepth;
        Int2 _fullSize;

        private GPUReadWriteImageBuffer _renderbuffer;
        private ReadOnlyBuffer<Triangle> _geometryBuffer;
        private ReadOnlyBuffer<BVHNode> _bvhHeap;

        private ReadWriteTexture3D<int> _bvhStack;
        private ReadWriteTexture2D<Float4> _attenuationStack;
        private ReadWriteTexture2D<Float4> _colorStack;

        private ReadWriteBuffer<RayCast> _rayCastBuffer;
        private ReadWriteTexture2D<int> _materialBuffer;

        public RayTracer(CommonScene scene, Int2 size, GPUReadWriteImageBuffer renderBuffer)
        {
            _fullSize = size;

            SceneConverter converter = new SceneConverter(GraphicsDevice.Default);
            _scene = converter.ConvertScene(scene);
            _camera = FullCamera.Create(_scene.camera, (float)size.X / size.Y);

            _bvhHeap = converter.BVHHeap;
            _bvhDepth = converter.BVHDepth;
            _geometryBuffer = converter.GeometryBuffer;
            _renderbuffer = renderBuffer;
        }

        public void TraceBounces(Tile tile)
        {
            int samples = _scene.config.samples;

            GraphicsDevice.Default.For(tile.Width, tile.Height, new CameraCastShader(_scene, _camera, tile.Offset, _fullSize, _rayCastBuffer));
            GraphicsDevice.Default.For(tile.Width, tile.Width, new CollisionShader(_scene, _geometryBuffer, _bvhHeap, _bvhStack, _rayCastBuffer, _materialBuffer));
        }

        public void RenderTile(Tile tile)
        {
            int samples = _scene.config.samples;
            _bvhStack = GraphicsDevice.Default.AllocateReadWriteTexture3D<int>(tile.Width, tile.Height, samples);
            _rayCastBuffer = GraphicsDevice.Default.AllocateReadWriteBuffer<RayCast>(tile.Width * tile.Height);
            _attenuationStack = GraphicsDevice.Default.AllocateReadWriteTexture2D<Float4>(tile.Width, tile.Height);
            _colorStack = GraphicsDevice.Default.AllocateReadWriteTexture2D<Float4>(tile.Width, tile.Height);
            _materialBuffer = GraphicsDevice.Default.AllocateReadWriteTexture2D<int>(tile.Width, tile.Height);

            TraceBounces(tile);
        }
    }
}
