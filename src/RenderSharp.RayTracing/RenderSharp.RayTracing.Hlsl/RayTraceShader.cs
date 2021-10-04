using ComputeSharp;
using RenderSharp.RayTracing.HLSL.Components;
using RenderSharp.RayTracing.HLSL.Geometry;
using RenderSharp.RayTracing.HLSL.Materials;
using RenderSharp.RayTracing.HLSL.Rays;
using RenderSharp.RayTracing.HLSL.Skys;
using RenderSharp.RayTracing.HLSL.Utils;

namespace RenderSharp.RayTracing.HLSL
{
    /// <summary>
    /// An <see cref="IPixelShader{Float4}"/> that ray traces a scene to render.
    /// </summary>
    [AutoConstructor]
    public readonly partial struct RayTraceShader : IComputeShader
    {
        private readonly Scene scene;
        private readonly Int2 _fullSize;
        private readonly Int2 _offset;
        private readonly ReadWriteTexture2D<Float4> output;
        private readonly ReadOnlyBuffer<Sphere> geometry;
        private readonly ReadOnlyBuffer<Material> materials;

        public bool GetHit(Ray ray, out RayCast cast, out Material material)
        {
            cast.origin = 0;
            cast.normal = 0;
            cast.coefficient = 0;
            material.albedo = Float4.Zero;
            material.emission = Float4.Zero;
            material.roughness = 0;
            material.metallic = 0;

            bool hit = false;
            float closest = float.MaxValue;

            RayCast cacheCast;
            for (int i = 0; i < geometry.Length; i++)
            {
                Sphere sphere = geometry[i];
                if (Sphere.IsHit(sphere, closest, ray, out cacheCast, out int matId))
                {
                    hit = true;
                    closest = cacheCast.coefficient;
                    cast = cacheCast;
                    material = materials[matId];
                }
            }

            return hit;
        }

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
                if (GetHit(ray, out RayCast cast, out Material material))
                {
                    Material.Emit(material, out Float4 emission);
                    color += emission * cumAttenuation;

                    Material.Scatter(material, ray, cast, ref randState, out Float4 attenuation, out ray);
                    cumAttenuation *= attenuation;
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

        public void Execute()
        {
            // Image
            float aspectRatio = (float)_fullSize.X / _fullSize.Y;

            // Camera
            FullCamera camera = FullCamera.Create(scene.camera, aspectRatio);

            // Render
            Float4 color = Float4.Zero;
            for (int s = 0; s < scene.config.samples; s++)
            {
                int x = _offset.X + ThreadIds.X;
                int y = _offset.Y + ThreadIds.Y;
                uint randState = (uint)(x * 1973 + y * 9277 + s * 26699) | 1;
                float u = (x + RandUtils.RandomFloat(ref randState)) / _fullSize.X;
                float v = 1 - ((y + RandUtils.RandomFloat(ref randState)) / _fullSize.Y);
                Ray ray = FullCamera.CreateRay(camera, u, v, ref randState);
                color += BounceRay(scene, ray, ref randState);
            }

            output[ThreadIds.XY] = color / scene.config.samples;
        }
    }
}
