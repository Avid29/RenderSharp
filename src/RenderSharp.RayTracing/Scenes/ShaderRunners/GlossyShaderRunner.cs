using ComputeSharp;
using RenderSharp.RayTracing.GPU.Shaders.Materials;
using RenderSharp.RayTracing.Scenes.Materials;
using RenderSharp.RayTracing.Scenes.Rays;
using RenderSharp.Render.Tiles;
using System.Numerics;

namespace RenderSharp.RayTracing.Scenes.ShaderRunners
{
    public class GlossyShaderRunner : IShaderRunner
    {
        private int _matId;
        private GlossyMaterial _material;

        public GlossyShaderRunner(int matId, GlossyMaterial material)
        {
            _matId = matId;
            _material = material;
        }

        public void Run(Tile tile, Scene scene,
            ReadWriteBuffer<Ray> rayBuffer,
            ReadWriteBuffer<RayCast> rayCastBuffer,
            ReadWriteTexture2D<int> materialBuffer,
            ReadWriteTexture2D<Vector4> attenuationBuffer,
            ReadWriteTexture2D<Vector4> colorBuffer,
            ReadWriteTexture2D<uint> randStates)
        {
            GraphicsDevice.Default.For(
                tile.Width, tile.Height,
                new GlossyShader(
                    _matId, scene, tile.Offset, _material,
                    rayBuffer, rayCastBuffer, materialBuffer,
                    attenuationBuffer, colorBuffer, randStates));
        }
    }
}
