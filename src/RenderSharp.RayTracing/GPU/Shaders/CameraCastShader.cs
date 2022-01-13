using ComputeSharp;
using RenderSharp.RayTracing.Scenes;
using RenderSharp.RayTracing.Scenes.Cameras;
using RenderSharp.RayTracing.Scenes.Rays;
using RenderSharp.RayTracing.Utils;

namespace RenderSharp.RayTracing.GPU.Shaders
{
    /// <summary>
    /// A shader that genereates the intial rays from a Camera for a tile.
    /// </summary>
    [AutoConstructor]
    [EmbeddedBytecode(DispatchAxis.XY)]
    public partial struct CameraCastShader : IComputeShader
    {
        private readonly Scene scene;
        private readonly FullCamera camera;
        private readonly int2 offset;
        private readonly int2 fullSize;
        private readonly ReadWriteBuffer<Ray> rayBuffer;

        public void Execute()
        {
            int2 pos = ThreadIds.XY;
            int2 dis = DispatchSize.XY;
            int bPos = pos.X * dis.X + pos.Y;

            int x = offset.X + ThreadIds.X;
            int y = offset.Y + ThreadIds.Y;
            int s = scene.config.samples;
            uint randState = (uint)(x * 1973 + y * 9277 + s * 26699) | 1;

            float u = (x + RandUtils.RandomFloat(ref randState) / fullSize.X);
            float v = 1 - ((y + RandUtils.RandomFloat(ref randState)) / fullSize.Y);
            Ray ray = FullCamera.CreateRay(camera, u, v, ref randState);
            rayBuffer[bPos] = ray;
        }
    }
}
