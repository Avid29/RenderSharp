using ComputeSharp;
using RenderSharp.RayTracing.HLSL;
using RenderSharp.RayTracing.HLSL.BVH;
using RenderSharp.RayTracing.HLSL.Conversion;
using RenderSharp.WinUI.Renderer;
using System;
using CommonScene = RenderSharp.Common.Scenes.Scene;
using ShaderMaterial = RenderSharp.RayTracing.HLSL.Materials.Material;
using ShaderScene = RenderSharp.RayTracing.HLSL.Components.Scene;
using ShaderSphere = RenderSharp.RayTracing.HLSL.Geometry.Sphere;
using ShaderTriangle = RenderSharp.RayTracing.HLSL.Geometry.Triangle;

namespace RenderSharp.Renderer
{
    public sealed class ShaderRenderer : ITileRenderer
    {
        private ShaderScene _scene;
        private ReadOnlyBuffer<ShaderTriangle> _geometryBuffer;
        //private ReadOnlyBuffer<ShaderSphere> _sphereBuffer;
        private ReadOnlyBuffer<ShaderMaterial> _materialBuffer;
        private ReadOnlyBuffer<BVHNode> _bvhHeap;
        private int _bvhDepth;
        private readonly GraphicsDevice _gpu;

        // TODO: Replace RayTracerShader with a generic T for any renderer
        private readonly
            Func<ShaderScene, Int2, Int2,
                ReadWriteTexture2D<Float4>,
                ReadOnlyBuffer<ShaderTriangle>,
                //ReadOnlyBuffer<ShaderSphere>,
                ReadOnlyBuffer<ShaderMaterial>,
                ReadOnlyBuffer<BVHNode>,
                ReadWriteTexture3D<int>,
                RayTraceShader> _shaderFactory;

        public ShaderRenderer() : this(Gpu.Default)
        { }

        public ShaderRenderer(GraphicsDevice gpu)
        {
            _gpu = gpu;
            _shaderFactory = static (sce, sz, offset, output, geometries, materials, bvh, stack) =>
                new RayTraceShader(sce, sz, offset, output, geometries, materials, bvh, stack);
        }

        public void AllocateResources(CommonScene scene)
        {
            SceneConverter converter = new SceneConverter(_gpu);
            _scene = converter.ConvertScene(scene);

            _materialBuffer = converter.MaterialBuffer;
            _bvhHeap = converter.BVHHeap;
            _bvhDepth = converter.BVHDepth;
            _geometryBuffer = converter.GeometryBuffer;
        }

        public void Render(IReadWriteTexture2D<Float4> texture, Int2 size, Int2 offset)
        {
            Int2 fullSize = new Int2(texture.Width, texture.Height);
            var tile = _gpu.AllocateReadWriteTexture2D<Float4>(size.X, size.Y);
            var stack = _gpu.AllocateReadWriteTexture3D<int>(size.X, size.Y, _bvhDepth + 1);

            _gpu.For(tile.Width, tile.Height, _shaderFactory(_scene, fullSize, offset, tile, _geometryBuffer, _materialBuffer, _bvhHeap, stack));
            _gpu.ForEach(texture, new OverlayShader(offset, tile, texture));
        }
    }
}
