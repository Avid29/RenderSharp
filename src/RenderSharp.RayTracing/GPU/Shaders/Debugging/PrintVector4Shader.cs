using ComputeSharp;
using System.Numerics;

namespace RenderSharp.RayTracing.GPU.Shaders.Debugging
{
    /// <summary>
    /// A shader that copies a 2D Ray buffer to the output texture shader.
    /// </summary>
    [AutoConstructor]
    [EmbeddedBytecode(DispatchAxis.XY)]
    public partial struct PrintVector4Shader : IComputeShader
    {
        private readonly Int2 offset;
        private readonly ReadWriteTexture2D<Vector4> texture;
        private readonly ReadWriteTexture2D<Vector4> printTexture;

        public void Execute()
        {
            Int2 pos = ThreadIds.XY;
            printTexture[pos] = texture[pos];
        }
    }
}
