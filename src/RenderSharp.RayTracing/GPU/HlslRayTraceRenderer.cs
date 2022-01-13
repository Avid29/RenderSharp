using ComputeSharp;
using RenderSharp.Render;
using RenderSharp.RayTracing.GPU.Shaders;

namespace RenderSharp.RayTracing.GPU
{
    public class HlslRayTraceRenderer : IRenderer
    {
        public void Render(IReadWriteTexture2D<Float4> buffer)
        {
            GraphicsDevice.Default.For(buffer.Width, buffer.Height, new TestShader(buffer));
        }
    }
}
