using ComputeSharp;
using RenderSharp.RayTracing.CPU;
using RenderSharp.RayTracing.CPU.Conversion;
using CommonScene = RenderSharp.Common.Components.Scene;
using ShaderScene = RenderSharp.RayTracing.CPU.Components.Scene;

namespace RenderSharp.WinUI.Renderer
{
    public class CPURenderer : ITileRenderer
    {
        private ShaderScene _scene;
        private Int2 _fullSize;

        public void AllocateResources(CommonScene scene)
        {
            SceneConverter converter = new SceneConverter();
            _scene = converter.ConvertScene(scene);
        }

        public void Render(IReadWriteTexture2D<Float4> texture, Int2 size, Int2 offset)
        {
            _fullSize = new Int2(texture.Width, texture.Height);
            RayTracer rayTracer = new RayTracer(_fullSize, _scene);

            Float4[,] frame = rayTracer.Render(offset, size);
            ReadWriteTexture2D<Float4> gpuFrame = Gpu.Default.AllocateReadWriteTexture2D(frame);

            Gpu.Default.ForEach(texture, new OverlayShader(offset, gpuFrame, texture));
        }
    }
}
