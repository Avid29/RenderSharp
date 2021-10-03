using ComputeSharp;
using ComputeSharp.WinUI;
using RenderSharp.RayTracing.HLSL;
using System;

namespace RenderSharp.Renderer
{
    public sealed class ShaderRenderer : IShaderRunner
    {
        private readonly GraphicsDevice _gpu;

        // TODO: Replace RayTracerShader with a generic T for any renderer
        private readonly Func<RayTraceShader> _shaderFactory;

        public ShaderRenderer() : this(Gpu.Default)
        { }

        public ShaderRenderer(GraphicsDevice gpu)
        {
            _gpu = gpu;
            _shaderFactory = static () => default;
        }

        public void Execute(IReadWriteTexture2D<Float4> texture, TimeSpan timespan)
        {
            _gpu.ForEach(texture, _shaderFactory());
        }
    }
}
