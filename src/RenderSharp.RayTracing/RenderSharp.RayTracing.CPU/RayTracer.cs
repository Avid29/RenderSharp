using ComputeSharp;
using RenderSharp.Common.Devices.Buffers;
using RenderSharp.Common.Render.Tiles;
using RenderSharp.RayTracing.CPU.Scenes;
using RenderSharp.RayTracing.CPU.Scenes.Cameras;
using RenderSharp.RayTracing.CPU.Scenes.Materials;
using RenderSharp.RayTracing.CPU.Scenes.Rays;
using RenderSharp.RayTracing.CPU.Utils;
using System.Numerics;

namespace RenderSharp.RayTracing.CPU
{
    public class RayTracer
    {
        private Scene _scene;
        private FullCamera _camera;
        private CPUReadWriteImageBuffer _buffer;

        public RayTracer(Scene scene, CPUReadWriteImageBuffer buffer)
        {
            _scene = scene;
            _buffer = buffer;

            float aspectRatio = (float)_buffer.Width / _buffer.Height;
            _camera = new FullCamera(_scene.Camera, aspectRatio);
        }

        private Vector4 BounceRay(Ray ray, ref uint randState)
        {
            Vector4 color = Float4.Zero;
            Vector4 cumAttenuation = Vector4.One;

            // Bounce the ray around the scene iteratively
            for (int depth = 0; depth < _scene.Config.MaxBounces; depth++)
            {
                if (_scene.World.GetHit(ray, out RayCast cast, out IMaterial material))
                {
                    if (material == null)
                        break;

                    material.Emit(out Vector4 emission);
                    color += emission * cumAttenuation;

                    material.Scatter(ray, cast, ref randState, out Vector4 attenuation, out ray);
                    cumAttenuation *= attenuation;
                }
                else
                {
                    // No object was hit
                    // Therefore the sky was hit
                    color += cumAttenuation * _scene.World.Sky.GetColor(ray);
                    break;
                }
            }

            return color;
        }

        public Float4 Execute(Int2 pos, int sample, Vector4 color)
        {
            uint randState = (uint)(pos.X * 1973 + pos.Y * 9277 + sample * 26699) | 1;
            float u = (pos.X + RandUtils.RandomFloat(ref randState)) / _buffer.Width;
            float v = 1 - ((pos.Y + RandUtils.RandomFloat(ref randState)) / _buffer.Height);
            Ray ray = _camera.CreateRay(u, v, ref randState);
            color += BounceRay(ray, ref randState) / _scene.Config.Samples;

            return color;
        }

        public void RenderTile(Tile tile)
        {
            int x1 = tile.OffsetX;
            int y1 = tile.OffsetY;
            int x2 = tile.OffsetX + tile.Width;
            int y2 = tile.OffsetY + tile.Height;

            for (int s = 0; s < _scene.Config.Samples; s++)
            {
                for (int x = x1; x < x2; x++)
                {
                    for (int y = y1; y < y2; y++)
                    {
                        _buffer[x, y] = Execute(new Int2(x, y), s, _buffer[x, y]);
                    }
                }
            }
        }
    }
}
