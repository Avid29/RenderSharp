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
        RayTracer _rayTracer;

        public IReadWriteImageBuffer Buffer => _buffer;

        public bool IsCPU => false;

        public void Setup(CommonScene scene, int imageWidth, int imageHeight)
        {
            _buffer = new GPUReadWriteImageBuffer(imageWidth, imageHeight);

            _rayTracer = new RayTracer(scene, new Int2(imageWidth, imageHeight), _buffer);
        }

        public void RenderTile(Tile tile)
        {
            _rayTracer.RenderTile(tile);
        }
    }
}
