using ComputeSharp;
using RenderSharp.RayTracing.Scenes;
using RenderSharp.RayTracing.Scenes.Cameras;
using RenderSharp.RayTracing.Scenes.Rays;
using RenderSharp.RayTracing.Utils;

namespace RenderSharp.RayTracing.GPU.Shaders
{
    /// <summary>
    /// A shader that generates the initial rays from a Camera for a tile.
    /// </summary>
    [AutoConstructor]
    [EmbeddedBytecode(DispatchAxis.XY)]
    public partial struct CameraCastShader : IComputeShader
    {
        private readonly Scene scene;
        private readonly FullCamera camera;
        private readonly Int2 offset;
        private readonly Int2 fullSize;
        private readonly ReadWriteBuffer<Ray> rayBuffer;
        private readonly ReadWriteTexture2D<uint> randStates;

        public void Execute()
        {
            Int2 pos = ThreadIds.XY;
            Int2 dis = DispatchSize.XY;
            int bPos = pos.Y * dis.X + pos.X;
            uint randState = randStates[pos];

            int x = offset.X + pos.X;
            int y = offset.Y + pos.Y;
            float u = (x + RandUtils.RandomFloat(ref randState)) / fullSize.X;
            float v = 1 - ((y + RandUtils.RandomFloat(ref randState)) / fullSize.Y);

            Ray ray = FullCamera.CreateRay(camera, u, v, ref randState);
            rayBuffer[bPos] = ray;
            randStates[pos] = randState;
        }
    }
}
