using ComputeSharp;

namespace RenderSharp.RayTracing.GPU.Shaders
{
    [AutoConstructor]
    [EmbeddedBytecode(DispatchAxis.XY)]
    public partial struct TestShader : IComputeShader
    {
        private IReadWriteTexture2D<Float4> texture;

        public void Execute()
        {
            Int2 pos = ThreadIds.XY;
            Int2 dis = DispatchSize.XY;
            Float2 uv = new Float2(pos.X, pos.Y) / dis;

            texture[pos] = new Float4(uv, 0, 1);
        }
    }
}
