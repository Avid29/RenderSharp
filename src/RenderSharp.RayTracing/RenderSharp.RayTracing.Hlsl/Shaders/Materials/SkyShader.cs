using ComputeSharp;
using RenderSharp.RayTracing.HLSL.Scenes;
using RenderSharp.RayTracing.HLSL.Scenes.Rays;

namespace RenderSharp.RayTracing.HLSL.Shaders.Materials
{
    [AutoConstructor]
    [EmbeddedBytecode(DispatchAxis.XY)]
    public partial struct SkyShader : IComputeShader
    {
        private readonly Scene scene;
        private readonly Float4 albedo;

        private readonly ReadWriteBuffer<Ray> rayBuffer;
        private readonly ReadWriteBuffer<RayCast> rayCastBuffer;
        private readonly ReadWriteTexture2D<int> materialBuffer;

        private readonly ReadWriteTexture2D<Float4> attenuationBuffer;
        private readonly ReadWriteTexture2D<Float4> colorBuffer;

        public void Execute()
        {
            Int2 pos = ThreadIds.XY;
            if (materialBuffer[pos] != -1) return;

            Int2 dis = DispatchSize.XY;
            int bPos = pos.X * dis.X + pos.Y;

            Ray ray = rayBuffer[bPos];
            RayCast cast = rayCastBuffer[bPos];

            Float3 unitDirection = Hlsl.Normalize(ray.direction);
            float t = 0.5f * (unitDirection.Y + 1);
            Float4 rawColor = (1f - t) * Float4.One + t * albedo;

            Float4 attenuation = attenuationBuffer[bPos];
            colorBuffer[bPos] += attenuation * rawColor;
        }
    }
}
