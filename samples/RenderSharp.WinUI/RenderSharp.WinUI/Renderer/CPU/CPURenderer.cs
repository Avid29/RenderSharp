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
        private int currentBar = 0;
        private bool done = false;
        private const int BAR_COUNT = 10;

        public void AllocateResources(CommonScene scene)
        {
            SceneConverter converter = new SceneConverter();
            _scene = converter.ConvertScene(scene);
        }

        public void Execute(IReadWriteTexture2D<Float4> texture, TimeSpan timespan)
        {
            if (currentBar >= BAR_COUNT)
                done = true;

            if (done) return;

            Int2 size = new Int2(texture.Width, texture.Height);
            Int2 barSize = new Int2(size.X, size.Y / BAR_COUNT);
            RayTracer rayTracer = new RayTracer(size, _scene);

            Int2 offset = new Int2(0, barSize.Y * currentBar);
            Float4[,] frame = rayTracer.Render(offset, barSize);
            ReadWriteTexture2D<Float4> gpuFrame = Gpu.Default.AllocateReadWriteTexture2D(frame);

            Gpu.Default.ForEach(texture, new OverlayShader(offset, gpuFrame, texture));

            currentBar++;
        }
    }
}
