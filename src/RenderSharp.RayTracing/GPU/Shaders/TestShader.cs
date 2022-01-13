using ComputeSharp;

namespace RenderSharp.RayTracing.GPU.Shaders
{
    [AutoConstructor]
    [EmbeddedBytecode(DispatchAxis.XY)]
    public partial struct TestShader : IComputeShader
    {
        private readonly int2 offset;
        private readonly int2 fullsize;
        private readonly ReadWriteTexture2D<float4> texture;

        public void Execute()
        {
            int2 pos = ThreadIds.XY + offset;
            float2 uv = new float2(pos.X, pos.Y) / fullsize;

            texture[pos] = new float4(uv, 0, 1);
        }
    }
}
