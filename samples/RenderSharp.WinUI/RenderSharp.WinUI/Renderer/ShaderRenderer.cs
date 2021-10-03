using ComputeSharp;
using ComputeSharp.WinUI;
using RenderSharp.RayTracing.HLSL;
using RenderSharp.RayTracing.HLSL.Conversion;
using System;

using CommonScene = RenderSharp.Common.Components.Scene;
using ShaderScene = RenderSharp.RayTracing.HLSL.Components.Scene;
using ShaderSphere = RenderSharp.RayTracing.HLSL.Geometry.Sphere;
using ShaderMaterial = RenderSharp.RayTracing.HLSL.Materials.Material;

namespace RenderSharp.Renderer
{
    public sealed class ShaderRenderer : IShaderRunner, ISceneRenderer
    {
        private ShaderScene _scene;
        private ReadOnlyBuffer<ShaderSphere> _geometryBuffer;
        private ReadOnlyBuffer<ShaderMaterial> _materialBuffer;
        private readonly GraphicsDevice _gpu;

        // TODO: Replace RayTracerShader with a generic T for any renderer
        private readonly Func<ShaderScene, ReadOnlyBuffer<ShaderSphere>, ReadOnlyBuffer<ShaderMaterial>, RayTraceShader> _shaderFactory;

        public ShaderRenderer() : this(Gpu.Default)
        { }

        public ShaderRenderer(GraphicsDevice gpu)
        {
            _gpu = gpu;
            _shaderFactory = static (s, g, m) => new RayTraceShader(s, g, m);
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
            _gpu.ForEach(texture, _shaderFactory(_scene, _geometryBuffer, _materialBuffer));
        }
    }
}
