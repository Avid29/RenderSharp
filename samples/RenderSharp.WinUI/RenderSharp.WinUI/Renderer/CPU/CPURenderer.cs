using ComputeSharp;
using RenderSharp.RayTracing.CPU;
using RenderSharp.RayTracing.CPU.Conversion;
using RenderSharp.Renderer;
using System;
using CommonScene = RenderSharp.Common.Components.Scene;
using ShaderScene = RenderSharp.RayTracing.CPU.Components.Scene;

namespace RenderSharp.WinUI.Renderer
{
    public class CPURenderer : ISceneRenderer
    {
        private ShaderScene _scene;

        public void AllocateResources(CommonScene scene)
        {
            SceneConverter converter = new SceneConverter();
            _scene = converter.ConvertScene(scene);
        }

        public void Execute(IReadWriteTexture2D<Float4> texture, TimeSpan timespan)
        {
            RayTracer rayTracer = new RayTracer(_scene);
            Float4[,] frame = rayTracer.Render(new Int2(texture.Width, texture.Height));
            ReadOnlyTexture2D<Float4> gpuFrame = Gpu.Default.AllocateReadOnlyTexture2D(frame);

            Gpu.Default.ForEach(texture, new OverlayShader(Int2.Zero, gpuFrame, texture));
        }
    }
}
