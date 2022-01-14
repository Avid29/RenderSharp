using ComputeSharp;
using RenderSharp.RayTracing.Scenes;
using RenderSharp.RayTracing.Scenes.Rays;
using System.Numerics;

namespace RenderSharp.RayTracing.GPU.Shaders.Materials
{
    [AutoConstructor]
    [EmbeddedBytecode(DispatchAxis.XY)]
    public partial struct SkyShader : IComputeShader
    {
        private readonly Scene scene;
        private readonly int2 offset;
        private readonly float4 albedo;

        private readonly ReadWriteBuffer<Ray> rayBuffer;
        private readonly ReadWriteBuffer<RayCast> rayCastBuffer;
        private readonly ReadWriteTexture2D<int> materialBuffer;

        private readonly ReadWriteTexture2D<float4> attenuationBuffer;
        private readonly ReadWriteTexture2D<float4> colorBuffer;

        public void Execute()
        {
            int2 pos = ThreadIds.XY;
            if (materialBuffer[pos] != -1) return;

            int2 dis = DispatchSize.XY;
            int bPos = pos.X * dis.X + pos.Y;

            Ray ray = rayBuffer[bPos];
            RayCast cast = rayCastBuffer[bPos];

            Vector3 unitDirection = Vector3.Normalize(ray.direction);
            float t = 0.5f * (unitDirection.Y + 1);
            float4 rawColor = (1f - t) * float4.One + t * albedo;

            float4 attenuation = attenuationBuffer[pos];
            colorBuffer[pos + offset] += attenuation * rawColor;
            materialBuffer[pos] = -2;
        }
    }
}
