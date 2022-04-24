using ComputeSharp;
using RenderSharp.RayTracing.Scenes.Rays;
using RenderSharp.Render.Tiles;
using System.Numerics;

namespace RenderSharp.RayTracing.Scenes.ShaderRunners
{
    public interface IShaderRunner
    {
        void Run(
            Tile tile, Scene scene,
            ReadWriteBuffer<Ray> rayBuffer,
            ReadWriteBuffer<RayCast> rayCastBuffer,
            ReadWriteTexture2D<int> materialBuffer,
            ReadWriteTexture2D<Vector4> attenuationBuffer,
            ReadWriteTexture2D<Vector4> colorBuffer,
            ReadWriteTexture2D<uint> randStates);
    }
}
