using ComputeSharp;
using ComputeSharp.WinUI;
using RenderSharp.RayTracing.HLSL;
using RenderSharp.RayTracing.HLSL.Conversion;
using RenderSharp.RayTracing.HLSL.Rays;
using System;

using CommonScene = RenderSharp.Common.Components.Scene;
using ShaderScene = RenderSharp.RayTracing.HLSL.Components.Scene;

namespace RenderSharp.Renderer
{
    public sealed class ShaderRenderer : IShaderRunner, ISceneRenderer
    {
        private ShaderScene _scene;
        private readonly GraphicsDevice _gpu;

        // TODO: Replace RayTracerShader with a generic T for any renderer
        private readonly Func<ShaderScene, RayTraceShader> _shaderFactory;

        public ShaderRenderer() : this(Gpu.Default)
        { }

        public ShaderRenderer(GraphicsDevice gpu)
        {
            _gpu = gpu;
            _shaderFactory = static (m) => new RayTraceShader(m);
        }

        public void AllocateResources(CommonScene scene)
        {
            SceneConverter converter = new SceneConverter(_gpu);
            _scene = converter.ConvertScene(scene);
        }

        public void Execute(IReadWriteTexture2D<Float4> texture, TimeSpan timespan)
        {
            _gpu.ForEach(texture, _shaderFactory(_scene));
        }
    }
}
