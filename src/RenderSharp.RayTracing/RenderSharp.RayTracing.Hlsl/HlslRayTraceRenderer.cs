using ComputeSharp;
using RenderSharp.Common.Devices.Buffers;
using RenderSharp.Common.Render;
using RenderSharp.Common.Render.Tiles;
using RenderSharp.RayTracing.HLSL.Conversion;
using RenderSharp.RayTracing.HLSL.Scenes;
using RenderSharp.RayTracing.HLSL.Scenes.BVH;
using RenderSharp.RayTracing.HLSL.Scenes.Geometry;
using RenderSharp.RayTracing.HLSL.Scenes.Materials;
using CommonScene = RenderSharp.Common.Scenes.Scene;

namespace RenderSharp.RayTracing.HLSL
{
    public class HlslRayTraceRenderer : IRenderer
    {
        Scene _scene;
        private GPUReadWriteImageBuffer _buffer;
        private ReadOnlyBuffer<Triangle> _geometryBuffer;
        private ReadOnlyBuffer<Material> _materialBuffer;
        private ReadOnlyBuffer<BVHNode> _bvhHeap;
        private int _bvhDepth;
        Int2 _fullSize;

        public IReadWriteImageBuffer Buffer => _buffer;

        public void Setup(CommonScene scene, int imageWidth, int imageHeight)
        {
            SceneConverter converter = new SceneConverter(Gpu.Default);
            _scene = converter.ConvertScene(scene);

            _materialBuffer = converter.MaterialBuffer;
            _bvhHeap = converter.BVHHeap;
            _bvhDepth = converter.BVHDepth;
            _geometryBuffer = converter.GeometryBuffer;

            _fullSize = new Int2(imageWidth, imageHeight);
            _buffer = new GPUReadWriteImageBuffer(imageWidth, imageHeight);
        }

        public void RenderTile(Tile tile)
        {
            Int2 offset = tile.Offset;
            var stack = Gpu.Default.AllocateReadWriteTexture3D<int>(tile.Width, tile.Height, _bvhDepth + 1);

            Gpu.Default.For(tile.Width, tile.Height, new RayTraceShader(_scene, _fullSize, offset, _buffer.Buffer, _geometryBuffer, _materialBuffer, _bvhHeap, stack));
        }
    }
}
