using ComputeSharp;
using RenderSharp.RayTracing.Scenes.Rays;
using System.Numerics;

namespace RenderSharp.RayTracing.GPU.Shaders.Debugging
{
    /// <summary>
    /// A shader that copies a 2D Ray buffer to the output texture shader.
    /// </summary>
    [AutoConstructor]
    [EmbeddedBytecode(DispatchAxis.XY)]
    public partial struct PrintRayShader : IComputeShader
    {
        private readonly Int2 offset;
        private readonly ReadWriteBuffer<Ray> texture;
        private readonly ReadWriteTexture2D<Vector4> printTexture;

        public void Execute()
        {
            Int2 pos = ThreadIds.XY;
            Int2 dis = DispatchSize.XY;
            int bPos = pos.Y * dis.X + pos.X;
            Vector3 v3 = Vector3.Normalize(texture[bPos].direction) / 2 + (Vector3.One * .5f);
            printTexture[pos] = new Vector4(v3, 1);
        }
    }
}
