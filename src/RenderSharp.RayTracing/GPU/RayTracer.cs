using ComputeSharp;
using RenderSharp.Buffer;
using RenderSharp.RayTracing.Conversion;
using RenderSharp.RayTracing.GPU.Shaders;
using RenderSharp.RayTracing.GPU.Shaders.Materials;
using RenderSharp.RayTracing.Scenes;
using RenderSharp.RayTracing.Scenes.BVH;
using RenderSharp.RayTracing.Scenes.Cameras;
using RenderSharp.RayTracing.Scenes.Geometry;
using RenderSharp.RayTracing.Scenes.Materials;
using RenderSharp.RayTracing.Scenes.Rays;
using RenderSharp.RayTracing.Scenes.ShaderRunners;
using RenderSharp.Render.Tiles;
using System.Numerics;
using CommonScene = RenderSharp.Scenes.Scene;

namespace RenderSharp.RayTracing.GPU
{
    public class RayTracer
    {
        private Scene _scene;
        private FullCamera _camera;
        private Int2 _fullSize;
        private GPUReadWriteImageBuffer _buffer;

        private ReadOnlyBuffer<Triangle> _geometryBuffer;
        private ReadOnlyBuffer<BVHNode> _bvhBuffer;
        private IShaderRunner[] _shaders;
        private int _bvhDepth;

        public RayTracer(CommonScene scene, Int2 fullsize, GPUReadWriteImageBuffer buffer)
        {
            SceneConverter converter = new SceneConverter();
            _scene = converter.ConvertScene(scene);
            _geometryBuffer = GraphicsDevice.Default.AllocateReadOnlyBuffer(converter.GeometryBuffer);
            _bvhBuffer = GraphicsDevice.Default.AllocateReadOnlyBuffer(converter.BVHHeap);
            _shaders = converter.ShaderRunnerBuffer;

            _bvhDepth = converter.BVHDepth;
            _fullSize = fullsize;
            _buffer = buffer;

            _camera = FullCamera.Create(_scene.camera, (float)fullsize.X / fullsize.Y);
        }

        public void RenderTile(Tile tile)
        {
            // Allocate buffers
            ReadWriteTexture3D<int> bvhStack = GraphicsDevice.Default.AllocateReadWriteTexture3D<int>(tile.Width, tile.Height, _bvhDepth + 1);
            ReadWriteBuffer<Ray> rayBuffer = GraphicsDevice.Default.AllocateReadWriteBuffer<Ray>(tile.Width * tile.Height);
            ReadWriteBuffer<RayCast> rayCastBuffer = GraphicsDevice.Default.AllocateReadWriteBuffer<RayCast>(tile.Width * tile.Height);
            ReadWriteTexture2D<int> materialBuffer = GraphicsDevice.Default.AllocateReadWriteTexture2D<int>(tile.Width, tile.Height);
            ReadWriteTexture2D<Vector4> attenuationBuffer = GraphicsDevice.Default.AllocateReadWriteTexture2D<Vector4>(tile.Width, tile.Height);
            ReadWriteTexture2D<Vector4> colorBuffer = GraphicsDevice.Default.AllocateReadWriteTexture2D<Vector4>(tile.Width, tile.Height);
            ReadWriteTexture2D<uint> randStates = GraphicsDevice.Default.AllocateReadWriteTexture2D<uint>(tile.Width, tile.Height);

            for (int s = 0; s < _scene.config.samples; s++)
            {
                // Reuse the same buffers and reset the data for each sample
                GraphicsDevice.Default.For(tile.Width, tile.Height, new InitalizeShader(_scene, tile.Offset, s, materialBuffer, colorBuffer, attenuationBuffer, randStates));
                GraphicsDevice.Default.For(tile.Width, tile.Height, new CameraCastShader(_scene, _camera, tile.Offset, _fullSize, rayBuffer, randStates));

                for (int i = 0; i < _scene.config.maxBounces; i++)
                {
                    // Find collisions from the ray buffer and write the cast information to the cast buffer
                    GraphicsDevice.Default.For(tile.Width, tile.Height, new CollisionShader(_scene, _geometryBuffer, _bvhBuffer, bvhStack, rayBuffer, rayCastBuffer, materialBuffer));

                    // These shaders adjust the atteniation buffer and also scatter the ray, overwriting the ray in the ray buffer
                    foreach (var shader in _shaders)
                    {
                        shader.Run(tile, _scene, rayBuffer, rayCastBuffer, materialBuffer, attenuationBuffer, colorBuffer, randStates);
                    }

                    // Run the Sky shader (will also be checked dynamically)
                    GraphicsDevice.Default.For(tile.Width, tile.Height, new SkyShader(_scene, tile.Offset, new Vector4(0.5f, 0.7f, 1f, 1f), rayBuffer, rayCastBuffer, materialBuffer, attenuationBuffer, colorBuffer));
                }

                GraphicsDevice.Default.For(tile.Width, tile.Height, new SampleCopyShader(_scene, tile.Offset, colorBuffer, _buffer.Buffer));
            }
        }
    }
}
