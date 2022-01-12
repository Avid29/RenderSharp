using ComputeSharp;
using RenderSharp.RayTracing.HLSL.Scenes;
using RenderSharp.RayTracing.HLSL.Scenes.Cameras;
using RenderSharp.RayTracing.HLSL.Scenes.Rays;
using RenderSharp.RayTracing.HLSL.Utils;

namespace RenderSharp.RayTracing.HLSL.Shaders
{
    /// <summary>
    /// A shader that genereates the intial rays from a Camera for a tile.
    /// </summary>
    [AutoConstructor]
    public partial struct CameraCastShader : IComputeShader
    {
        private readonly Scene scene;
        private readonly FullCamera camera;
        private readonly Int2 offset;
        private readonly Int2 fullSize;
        private readonly ReadWriteBuffer<RayCast> rayCastBuffer;

        public void Execute()
        {
            Int2 pos = ThreadIds.XY;
            Int2 dis = DispatchSize.XY;
            int bPos = pos.X * dis.X + pos.Y;

            int x = offset.X + ThreadIds.X;
            int y = offset.Y + ThreadIds.Y;
            int s = scene.config.samples;
            uint randState = (uint)(x * 1973 + y * 9277 + s * 26699) | 1;

            float u = (x + RandUtils.RandomFloat(ref randState) / fullSize.X);
            float v = 1 - ((y + RandUtils.RandomFloat(ref randState)) / fullSize.Y);
            RayCast ray = FullCamera.CreateRay(camera, u, v, ref randState);
            rayCastBuffer[bPos] = ray;
        }
    }
}
