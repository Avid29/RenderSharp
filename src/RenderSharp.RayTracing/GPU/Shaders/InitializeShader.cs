using ComputeSharp;
using RenderSharp.RayTracing.Scenes;
using System.Numerics;

namespace RenderSharp.RayTracing.GPU.Shaders
{
    [AutoConstructor]
    [EmbeddedBytecode(DispatchAxis.XY)]
    public partial struct InitalizeShader : IComputeShader
    {
        private readonly Scene scene;
        private readonly int2 offset;
        private readonly int sample;

        private readonly ReadWriteTexture2D<int> materialBuffer;
        private readonly ReadWriteTexture2D<Vector4> colorBuffer;
        private readonly ReadWriteTexture2D<Vector4> attenuationBuffer;
        private readonly ReadWriteTexture2D<uint> randStates;

        public void Execute()
        {
            int2 pos = ThreadIds.XY;
            int x = offset.X + ThreadIds.X;
            int y = offset.Y + ThreadIds.Y;
            int s = sample;

            materialBuffer[pos] = 0;
            colorBuffer[pos] = Vector4.Zero;
            attenuationBuffer[pos] = Vector4.One;
            randStates[pos] = (uint)(x * 1973 + y * 9277 + s * 266999) | 1;
        }
    }
}
