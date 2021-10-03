using ComputeSharp;
using RenderSharp.Common.Components;
using RenderSharp.RayTracing.CPU;
using RenderSharp.Renderer;
using System;

namespace RenderSharp.WinUI.Renderer
{
    public class CPURenderer : ISceneRenderer
    {
        public void AllocateResources(Scene scene) { }

        public void Execute(IReadWriteTexture2D<Float4> texture, TimeSpan timespan)
        {
            RayTracer rayTracer = new RayTracer();
            Float4[,] frame = rayTracer.Render(new Int2(texture.Width, texture.Height));
            ReadOnlyTexture2D<Float4> gpuFrame = Gpu.Default.AllocateReadOnlyTexture2D(frame);

            Gpu.Default.ForEach(texture, new CopyShader(gpuFrame));
        }
    }
}
