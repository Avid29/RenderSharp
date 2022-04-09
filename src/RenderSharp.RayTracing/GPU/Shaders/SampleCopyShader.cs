using ComputeSharp;
using RenderSharp.RayTracing.Scenes;
using System.Numerics;

namespace RenderSharp.RayTracing.GPU.Shaders
{
    [AutoConstructor]
    [EmbeddedBytecode(DispatchAxis.XY)]
    public partial struct SampleCopyShader : IComputeShader
    {
        private readonly Scene scene;
        private readonly int2 offset;

        private readonly ReadWriteTexture2D<Vector4> colorBuffer;
        private readonly ReadWriteTexture2D<Vector4> outputBuffer;

        public void Execute()
        {
            int2 pos = ThreadIds.XY;
            outputBuffer[pos + offset] += colorBuffer[pos] / scene.config.samples;
        }
    }
}
