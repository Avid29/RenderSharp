using ComputeSharp;
using RenderSharp.RayTracing.HLSL;
using RenderSharp.RayTracing.HLSL.Conversion;
using RenderSharp.WinUI.Renderer;
using System;
using CommonScene = RenderSharp.Common.Components.Scene;
using ShaderMaterial = RenderSharp.RayTracing.HLSL.Materials.Material;
using ShaderScene = RenderSharp.RayTracing.HLSL.Components.Scene;
using ShaderSphere = RenderSharp.RayTracing.HLSL.Geometry.Sphere;

namespace RenderSharp.Renderer
{
    public sealed class ShaderRenderer : ISceneRenderer
    {
        private ShaderScene _scene;
        private ReadOnlyBuffer<ShaderSphere> _geometryBuffer;
        private ReadOnlyBuffer<ShaderMaterial> _materialBuffer;
        private readonly GraphicsDevice _gpu;

        // TODO: Replace RayTracerShader with a generic T for any renderer
        private readonly
            Func<ShaderScene, Int2, Int2,
                ReadWriteTexture2D<Float4>,
                ReadOnlyBuffer<ShaderSphere>,
                ReadOnlyBuffer<ShaderMaterial>,
                RayTraceShader> _shaderFactory;

        public ShaderRenderer() : this(Gpu.Default)
        { }

        public ShaderRenderer(GraphicsDevice gpu)
        {
            _gpu = gpu;
            _shaderFactory = static (s, f, t, o, g, m) => new RayTraceShader(s, f, t, o, g, m);
        }

        public void AllocateResources(CommonScene scene)
        {
            SceneConverter converter = new SceneConverter(_gpu);
            _scene = converter.ConvertScene(scene);
            if (converter.IsGeomertyLoaded) _geometryBuffer = converter.GeometryBuffer;
            if (converter.AreMaterialsLoaded) _materialBuffer = converter.MaterialBuffer;
        }

        public void Execute(IReadWriteTexture2D<Float4> texture, TimeSpan timespan)
        {
            Int2 size = new Int2(texture.Width, texture.Height);
            var tile = _gpu.AllocateReadWriteTexture2D<Float4>(texture.Width, texture.Height);
            Int2 offset = new Int2(0, 0);

            _gpu.For(tile.Width, tile.Height, _shaderFactory(_scene, size, offset, tile, _geometryBuffer, _materialBuffer));
            _gpu.ForEach(texture, new OverlayShader(offset, tile, texture));
        }
    }
}
