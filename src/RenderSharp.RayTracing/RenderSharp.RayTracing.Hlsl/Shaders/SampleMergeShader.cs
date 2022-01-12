using ComputeSharp;

namespace RenderSharp.RayTracing.HLSL
{
    [AutoConstructor]
    [EmbeddedBytecode(DispatchAxis.XY)]
    public readonly partial struct SampleMergeShader : IComputeShader
    {
        private readonly int samples;
        private readonly Int2 offset;
        private readonly ReadWriteTexture2D<Float4> output;
        private readonly ReadWriteTexture3D<Float4> colorStack;

        public void Execute()
        {
            Int2 pos = ThreadIds.XY;

            for (int s = 0; s < samples; s++)
            {
                output[pos + offset] += colorStack[new Int3(pos, s)];
            }

            output[pos + offset] /= samples;
        }
    }
}
