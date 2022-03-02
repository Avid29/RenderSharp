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
using RenderSharp.Render.Tiles;
using RenderSharp.Utils.Shaders;
using System.Numerics;
using CommonScene = RenderSharp.Scenes.Scene;

namespace RenderSharp.RayTracing.GPU
{
    public class RayTracer
    {
        private Scene _scene;
        private FullCamera _camera;
        private int2 _fullSize;
        private GPUReadWriteImageBuffer _buffer;

        private ReadOnlyBuffer<Triangle> _geometryBuffer;
        private ReadOnlyBuffer<BVHNode> _bvhBuffer;
        private int _bvhDepth;

        public RayTracer(CommonScene scene, int2 fullsize, GPUReadWriteImageBuffer buffer)
        {
            SceneConverter converter = new SceneConverter();
            _scene = converter.ConvertScene(scene);
            _geometryBuffer = GraphicsDevice.Default.AllocateReadOnlyBuffer(converter.GeometryBuffer);
            _bvhBuffer = GraphicsDevice.Default.AllocateReadOnlyBuffer(converter.BVHHeap);

            _bvhDepth = converter.BVHDepth;
            _fullSize = fullsize;
            _buffer = buffer;

            _camera = FullCamera.Create(_scene.camera, (float)fullsize.X / fullsize.Y);
        }

        public void RenderTile(Tile tile)
        {
            // TODO: Multiple samples

            // Allocate buffers
            ReadWriteTexture3D<int> bvhStack = GraphicsDevice.Default.AllocateReadWriteTexture3D<int>(tile.Width, tile.Height, _bvhDepth + 1);
            ReadWriteBuffer<Ray> rayBuffer = GraphicsDevice.Default.AllocateReadWriteBuffer<Ray>(tile.Width * tile.Height);
            ReadWriteBuffer<RayCast> rayCastBuffer = GraphicsDevice.Default.AllocateReadWriteBuffer<RayCast>(tile.Width * tile.Height);
            ReadWriteTexture2D<int> materialBuffer = GraphicsDevice.Default.AllocateReadWriteTexture2D<int>(tile.Width, tile.Height);
            ReadWriteTexture2D<Vector4> attenuationBuffer = GraphicsDevice.Default.AllocateReadWriteTexture2D<Vector4>(tile.Width, tile.Height);
            ReadWriteTexture2D<uint> randStates = GraphicsDevice.Default.AllocateReadWriteTexture2D<uint>(tile.Width, tile.Height);

            GraphicsDevice.Default.For(tile.Width, tile.Height, new InitalizeShader(_scene, tile.Offset, attenuationBuffer, randStates));
            GraphicsDevice.Default.For(tile.Width, tile.Height, new CameraCastShader(_scene, _camera, tile.Offset, _fullSize, rayBuffer, randStates));

            // Render each object with this diffuse material.
            DiffuseMaterial diffuse = DiffuseMaterial.Create(float4.One * .8f, .5f);

            for (int i = 0; i < _scene.config.maxBounces; i++)
            {
                // Find collisions from the ray buffer and write the cast information to the cast buffer
                GraphicsDevice.Default.For(tile.Width, tile.Height, new CollisionShader(_scene, _geometryBuffer, _bvhBuffer, bvhStack, rayBuffer, rayCastBuffer, materialBuffer));

                // TODO: Check which materials were hit and dyanmically select the shader(s) to run
                // These shaders also scatter the ray, overwriting the ray in the ray buffer
                GraphicsDevice.Default.For(tile.Width, tile.Height, new DiffuseShader(0, _scene, tile.Offset, diffuse, rayBuffer, rayCastBuffer, materialBuffer, attenuationBuffer, _buffer.Buffer, randStates));

                // Run the Sky shader (will also be checked dynamically)
                GraphicsDevice.Default.For(tile.Width, tile.Height, new SkyShader(_scene, tile.Offset, new Vector4(0.5f, 0.7f, 1f, 1f), rayBuffer, rayCastBuffer, materialBuffer, attenuationBuffer, _buffer.Buffer));
            }
        }
    }
}
