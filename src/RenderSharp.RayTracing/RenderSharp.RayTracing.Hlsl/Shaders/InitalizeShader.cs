using ComputeSharp;
using RenderSharp.RayTracing.HLSL.Scenes;

namespace RenderSharp.RayTracing.HLSL.Shaders
{
    [AutoConstructor]
    [EmbeddedBytecode(DispatchAxis.XY)]
    public partial struct InitalizeShader : IComputeShader
    {
        private readonly Scene scene;
        private readonly Int2 offset;

        private readonly ReadWriteTexture2D<float4> attenuationBuffer;
        private readonly ReadWriteTexture2D<uint> randStates;

        public void Execute()
        {
            Int2 pos = ThreadIds.XY;
            int x = offset.X + ThreadIds.X;
            int y = offset.Y + ThreadIds.Y;
            int s = scene.config.samples;

            attenuationBuffer[pos] = Float4.One;
            randStates[pos] = (uint)(x * 1973 + y * 9277 + s * 26699) | 1;
        }
    }
}
