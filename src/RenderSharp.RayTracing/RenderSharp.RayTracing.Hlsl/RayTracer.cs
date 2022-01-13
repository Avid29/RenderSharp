using ComputeSharp;
using RenderSharp.Common.Devices.Buffers;
using RenderSharp.Common.Render.Tiles;
using RenderSharp.RayTracing.HLSL.Conversion;
using RenderSharp.RayTracing.HLSL.Scenes;
using RenderSharp.RayTracing.HLSL.Scenes.BVH;
using RenderSharp.RayTracing.HLSL.Scenes.Cameras;
using RenderSharp.RayTracing.HLSL.Scenes.Geometry;
using RenderSharp.RayTracing.HLSL.Scenes.Materials;
using RenderSharp.RayTracing.HLSL.Scenes.Rays;
using RenderSharp.RayTracing.HLSL.Shaders;
using RenderSharp.RayTracing.HLSL.Shaders.Materials;
using CommonScene = RenderSharp.Common.Scenes.Scene;

namespace RenderSharp.RayTracing.HLSL
{
    public class RayTracer
    {
        private Scene _scene;
        private FullCamera _camera;
        private int _bvhDepth;
        Int2 _fullSize;

        private GPUReadWriteImageBuffer _renderbuffer;
        private ReadOnlyBuffer<Triangle> _geometryBuffer;
        private ReadOnlyBuffer<BVHNode> _bvhHeap;

        private ReadWriteTexture3D<int> _bvhStack;
        private ReadWriteTexture2D<Float4> _attenuationBuffer;
        private ReadWriteTexture2D<Float4> _colorBuffer;

        private ReadWriteBuffer<Ray> _rayBuffer;
        private ReadWriteBuffer<RayCast> _rayCastBuffer;
        private ReadWriteTexture2D<int> _materialBuffer;
        private ReadWriteTexture2D<uint> _randStates;

        public RayTracer(CommonScene scene, Int2 size, GPUReadWriteImageBuffer renderBuffer)
        {
            _fullSize = size;

            SceneConverter converter = new SceneConverter(GraphicsDevice.Default);
            _scene = converter.ConvertScene(scene);
            _camera = FullCamera.Create(_scene.camera, (float)size.X / size.Y);

            _bvhHeap = converter.BVHHeap;
            _bvhDepth = converter.BVHDepth;
            _geometryBuffer = converter.GeometryBuffer;
            _renderbuffer = renderBuffer;
        }

        public void TraceBounces(Tile tile)
        {
            // TODO: Multiple samples

            GraphicsDevice.Default.For(tile.Width, tile.Height, new InitalizeShader(_scene, tile.Offset, _attenuationBuffer, _randStates));
            GraphicsDevice.Default.For(tile.Width, tile.Height, new CameraCastShader(_scene, _camera, tile.Offset, _fullSize, _rayBuffer));
            
            // Render each object with this diffuse material.
            DiffuseMaterial diffuse = DiffuseMaterial.Create(float4.One * .8f, .5f);

            for (int i = 0; i < _scene.config.maxBounces; i++)
            {
                // Find collisions from the ray buffer and write the cast information to the cast buffer
                GraphicsDevice.Default.For(tile.Width, tile.Width, new CollisionShader(_scene, _geometryBuffer, _bvhHeap, _bvhStack, _rayBuffer, _rayCastBuffer, _materialBuffer));

                // TODO: Check which materials were hit and dyanmically select the shader(s) to run
                // These shaders also scatter the ray, overwriting the ray in the ray buffer
                GraphicsDevice.Default.For(tile.Width, tile.Height, new DiffuseShader(0, _scene, diffuse, _rayBuffer, _rayCastBuffer, _materialBuffer, _attenuationBuffer, _colorBuffer, _randStates));

                // Run the Sky shader (will also be checked dynamically)
                GraphicsDevice.Default.For(tile.Width, tile.Height, new SkyShader(_scene, new Float4(0.5f, 0.7f, 1f, 1f), _rayBuffer, _rayCastBuffer, _materialBuffer, _attenuationBuffer, _colorBuffer));
            }
        }

        public void RenderTile(Tile tile)
        {
            int samples = _scene.config.samples;
            _bvhStack = GraphicsDevice.Default.AllocateReadWriteTexture3D<int>(tile.Width, tile.Height, _bvhDepth + 1);
            _rayBuffer = GraphicsDevice.Default.AllocateReadWriteBuffer<Ray>(tile.Width * tile.Height);
            _rayCastBuffer = GraphicsDevice.Default.AllocateReadWriteBuffer<RayCast>(tile.Width * tile.Height);
            _attenuationBuffer = GraphicsDevice.Default.AllocateReadWriteTexture2D<Float4>(tile.Width, tile.Height);
            _colorBuffer = GraphicsDevice.Default.AllocateReadWriteTexture2D<Float4>(tile.Width, tile.Height);
            _materialBuffer = GraphicsDevice.Default.AllocateReadWriteTexture2D<int>(tile.Width, tile.Height);
            _randStates = GraphicsDevice.Default.AllocateReadWriteTexture2D<uint>(tile.Width, tile.Height);

            TraceBounces(tile);
        }
    }
}
