using ComputeSharp;
using RenderSharp.RayTracing.CPU.Components;
using RenderSharp.RayTracing.CPU.Materials;
using RenderSharp.RayTracing.CPU.Rays;
using RenderSharp.RayTracing.CPU.Utils;
using System.Numerics;
using System.Threading.Tasks;

namespace RenderSharp.RayTracing.CPU
{
    public class RayTracer
    {
        private Scene _scene;
        private Int2 _fullSize;

        public RayTracer(Int2 size, Scene scene)
        {
            _scene = scene;
            _fullSize = size;
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

        public Float4 Execute(Int2 pos)
        {
            // Image
            float aspectRatio = (float)_fullSize.X / _fullSize.Y;

            // Camera
            FullCamera camera = new FullCamera(_scene.Camera, aspectRatio);

            // Render
            Vector4 color = Vector4.Zero;
            for (int s = 0; s < _scene.Config.Samples; s++)
            {
                uint randState = (uint)(pos.X * 1973 + pos.Y * 9277 + s * 26699) | 1;
                float u = (pos.X + RandUtils.RandomFloat(ref randState)) / _fullSize.X;
                float v = 1 - ((pos.Y + RandUtils.RandomFloat(ref randState)) / _fullSize.Y);
                Ray ray = camera.CreateRay(u, v, ref randState);
                color += BounceRay(ray, ref randState);
            }

            return color / _scene.Config.Samples;
        }

        public Float4[,] Render(Int2 offset, Int2 size)
        {
            Float4[,] frame = new Float4[size.Y, size.X];
            Parallel.For(0, size.X, x =>
            {
                for (int y = 0; y < size.Y; y++)
                {
                    frame[y,x] = Execute(new Int2(offset.X + x, offset.Y + y));
                }
            });

            return frame;
        }
    }
}
