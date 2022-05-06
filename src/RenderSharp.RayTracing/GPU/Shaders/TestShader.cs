using ComputeSharp;

namespace RenderSharp.RayTracing.GPU.Shaders
{
    [AutoConstructor]
    [EmbeddedBytecode(DispatchAxis.XY)]
    public partial struct TestShader : IComputeShader
    {
        private readonly Int2 offset;
        private readonly Int2 fullsize;
        private readonly ReadWriteTexture2D<Float4> texture;

        public void Execute()
        {
            Int2 pos = ThreadIds.XY + offset;
            Float2 uv = new Float2(pos.X, pos.Y) / fullsize;

            texture[pos] = new Float4(uv, 0, 1);
        }
    }
}
