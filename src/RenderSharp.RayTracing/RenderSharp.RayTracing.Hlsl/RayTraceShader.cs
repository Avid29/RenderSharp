﻿using ComputeSharp;
using RenderSharp.RayTracing.HLSL.Components;
using RenderSharp.RayTracing.HLSL.Geometry;
using RenderSharp.RayTracing.HLSL.Rays;
using RenderSharp.RayTracing.HLSL.Skys;
using RenderSharp.RayTracing.HLSL.Utils;

namespace RenderSharp.RayTracing.HLSL
{
    /// <summary>
    /// An <see cref="IPixelShader{Float4}"/> that ray traces a scene to render.
    /// </summary>
    [AutoConstructor]
    public readonly partial struct RayTraceShader : IPixelShader<Float4>
    {
        private readonly Scene scene;

        /// <summary>
        /// Bounces a ray around a <see cref="Scene"/>.
        /// </summary>
        /// <param name="scene">The scene to bounce the ray in.</param>
        /// <param name="ray">The original ray to bounce.</param>
        /// <param name="randState">A integer used through out the shader to provide a random number.</param>
        /// <returns>The color of pixel from the original ray.</returns>
        private Float4 BounceRay(Scene scene, Ray ray, ref uint randState)
        {
            Float4 color = Float4.Zero;
            Float4 cumAttenuation = Float4.One;

            // Bounce the ray around the scene iteratively
            for (int depth = 0; depth < scene.config.maxBounces; depth++)
            {
                Sphere sphere;
                sphere.center = Float3.Zero;
                sphere.radius = 0.5f;

                if (Sphere.IsHit(sphere, ray, out RayCast cast))
                {
                    Float3 target = cast.origin + cast.normal + RandUtils.RandomInUnitSphere(ref randState);
                    cumAttenuation *= new Float4(Float3.One * 0.5f, 1);
                    ray = Ray.Create(cast.origin, target - cast.origin);
                }
                else
                {
                    // No object was hit
                    // Therefore the sky was hit
                    color += cumAttenuation * Sky.Color(scene.world.sky, ray);
                    break;
                }
            }

            return color;
        }

        public Float4 Execute()
        {
            // Image
            Float2 size = DispatchSize.XY;
            float aspectRatio = size.X / size.Y;

            // Camera

            // Render
            Float4 color = Float4.Zero;
            for (int s = 0; s < scene.config.samples; s++)
            {
                uint randState = (uint)(ThreadIds.X * 1973 + ThreadIds.Y * 9277 + s * 26699) | 1;
                float u = (ThreadIds.X + RandUtils.RandomFloat(ref randState)) / size.X;
                float v = 1 - ((ThreadIds.Y + RandUtils.RandomFloat(ref randState)) / size.Y);
                Ray ray = Camera.CreateRay(scene.camera, u, v);
                color += BounceRay(scene, ray, ref randState);
            }
            return color / scene.config.samples;
        }
    }
}
