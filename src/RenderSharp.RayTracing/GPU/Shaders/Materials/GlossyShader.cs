using ComputeSharp;
using RenderSharp.RayTracing.Scenes;
using RenderSharp.RayTracing.Scenes.Materials;
using RenderSharp.RayTracing.Scenes.Rays;
using RenderSharp.RayTracing.Utils;

namespace RenderSharp.RayTracing.GPU.Shaders.Materials
{
    [AutoConstructor]
    [EmbeddedBytecode(DispatchAxis.XY)]
    public partial struct GlossyShader : IComputeShader
    {
        private readonly int matId;

        private readonly Scene scene;
        private readonly GlossyMaterial material;

        private readonly ReadWriteBuffer<Ray> rayBuffer;
        private readonly ReadWriteBuffer<RayCast> rayCastBuffer;
        private readonly ReadWriteTexture2D<int> materialBuffer;

        private readonly ReadWriteTexture2D<float4> attenuationBuffer;
        private readonly ReadWriteTexture2D<float4> colorBuffer;
        private readonly ReadWriteTexture2D<uint> randStates;

        public void Execute()
        {
            int2 pos = ThreadIds.XY;

            // This ray didn't hit an object with this material
            if (materialBuffer[pos] != matId) return;

            int2 dis = DispatchSize.XY;
            int bPos = pos.X * dis.X + pos.Y;
            uint randState = randStates[pos];

            Ray ray = rayBuffer[bPos];
            RayCast cast = rayCastBuffer[bPos];

            float3 target = Hlsl.Reflect(Hlsl.Normalize(ray.direction), cast.normal);
            target += material.roughness * RandUtils.RandomInUnitSphere(ref randState);
            Ray scatter = Ray.Create(cast.origin, target - cast.origin);

            rayBuffer[bPos] = scatter;
            attenuationBuffer[pos] *= material.albedo;
            randStates[pos] = randState;
        }
    }
}
