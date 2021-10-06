using ComputeSharp;
using RenderSharp.Common.Devices.Buffers;
using RenderSharp.Common.Render.Tiles;
using RenderSharp.RayTracing.HLSL.Conversion;
using RenderSharp.RayTracing.HLSL.Scenes;
using RenderSharp.RayTracing.HLSL.Scenes.BVH;
using RenderSharp.RayTracing.HLSL.Scenes.Geometry;
using RenderSharp.RayTracing.HLSL.Scenes.Materials;
using CommonScene = RenderSharp.Common.Scenes.Scene;

namespace RenderSharp.RayTracing.HLSL
{
    public class RayTracer
    {
        private Scene _scene;
        private GPUReadWriteImageBuffer _buffer;
        private ReadOnlyBuffer<Triangle> _geometryBuffer;
        private ReadOnlyBuffer<Material> _materialBuffer;
        private ReadOnlyBuffer<BVHNode> _bvhHeap;
        private int _bvhDepth;
        Int2 _fullSize;

        public RayTracer(CommonScene scene, Int2 size, GPUReadWriteImageBuffer buffer)
        {
            _fullSize = size;

            SceneConverter converter = new SceneConverter(Gpu.Default);
            _scene = converter.ConvertScene(scene);

            _materialBuffer = converter.MaterialBuffer;
            _bvhHeap = converter.BVHHeap;
            _bvhDepth = converter.BVHDepth;
            _geometryBuffer = converter.GeometryBuffer;
            _buffer = buffer;
        }

        public void TraceBounces(Tile tile)
        {
            var bvhStack = Gpu.Default.AllocateReadWriteTexture3D<int>(tile.Width, tile.Height, _bvhDepth + 1);
            var attenuationStack = Gpu.Default.AllocateReadWriteTexture2D<Float4>(tile.Width, tile.Height);
            var colorStack = Gpu.Default.AllocateReadWriteTexture2D<Float4>(tile.Width, tile.Height);
            for (int s = 0; s < _scene.config.samples; s++)
            {
                Gpu.Default.For(tile.Width, tile.Height,
                    new PathTraceShader(_scene, _fullSize, tile.Offset, s, _buffer.Buffer, _geometryBuffer, _materialBuffer, _bvhHeap, bvhStack, attenuationStack, colorStack));
            }
        }

        public void RenderTile(Tile tile)
        {
            TraceBounces(tile);
        }
    }
}
