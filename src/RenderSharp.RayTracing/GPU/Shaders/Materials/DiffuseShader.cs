using ComputeSharp;
using RenderSharp.RayTracing.Scenes;
using RenderSharp.RayTracing.Scenes.Materials;
using RenderSharp.RayTracing.Scenes.Rays;
using RenderSharp.RayTracing.Utils;
using System.Numerics;

namespace RenderSharp.RayTracing.GPU.Shaders.Materials
{
    [AutoConstructor]
    [EmbeddedBytecode(DispatchAxis.XY)]
    public partial struct DiffuseShader : IComputeShader
    {
        private readonly int matId;

        private readonly Scene scene;
        private readonly Int2 offset;
        private readonly DiffuseMaterial material;

        private readonly ReadWriteBuffer<Ray> rayBuffer;
        private readonly ReadWriteBuffer<RayCast> rayCastBuffer;
        private readonly ReadWriteTexture2D<int> materialBuffer;

        private readonly ReadWriteTexture2D<Vector4> attenuationBuffer;
        private readonly ReadWriteTexture2D<Vector4> colorBuffer;
        private readonly ReadWriteTexture2D<uint> randStates;

        public void Execute()
        {
            Int2 pos = ThreadIds.XY;

            // This ray didn't hit an object with this material
            if (materialBuffer[pos] != matId) return;

            Int2 dis = DispatchSize.XY;
            int bPos = pos.Y * dis.X + pos.X;
            uint randState = randStates[pos];

            //Ray ray = rayBuffer[bPos];
            RayCast cast = rayCastBuffer[bPos];

            Vector3 target = cast.origin + cast.normal;
            target += material.roughness * RandUtils.RandomInUnitSphere(ref randState);
            Ray scatter = Ray.Create(cast.origin, target - cast.origin);

            rayBuffer[bPos] = scatter;
            attenuationBuffer[pos] *= material.albedo;
            randStates[pos] = randState;
        }
    }
}
