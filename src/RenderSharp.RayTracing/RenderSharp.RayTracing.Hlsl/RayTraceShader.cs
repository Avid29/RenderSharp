using ComputeSharp;
using RenderSharp.RayTracing.HLSL.Scenes;
using RenderSharp.RayTracing.HLSL.Scenes.BVH;
using RenderSharp.RayTracing.HLSL.Scenes.Cameras;
using RenderSharp.RayTracing.HLSL.Scenes.Geometry;
using RenderSharp.RayTracing.HLSL.Scenes.Materials;
using RenderSharp.RayTracing.HLSL.Scenes.Rays;
using RenderSharp.RayTracing.HLSL.Scenes.Skys;
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
        private readonly ReadOnlyBuffer<Triangle> geometry;
        private readonly ReadOnlyBuffer<Material> materials;
        private readonly ReadOnlyBuffer<BVHNode> bvhHeap;
        private readonly ReadWriteTexture3D<int> bvhStack;

        public bool GetHit(Ray ray, out RayCast cast, out Material material, Int2 pos)
        {
            return GetHitWithBVH(ray, out cast, out material, pos);
        }

        public bool GetHitWithBVH(Ray ray, out RayCast cast, out Material material, Int2 pos)
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

            // Init stack with root BVH
            int stackIndex = 0;
            bvhStack[pos.X, pos.Y, 0] = 0;

            do
            {
                // Pop the stack
                int nodeIndex = bvhStack[pos.X, pos.Y, stackIndex--];
                BVHNode node = bvhHeap[nodeIndex];

                if (BVHNode.IsHit(node, ray, closest))
                {
                    if (node.geoI != -1)
                    {
                        // Leaf node
                        Triangle triange = geometry[node.geoI];
                        if (Triangle.IsHit(triange, ray, out cacheCast))
                        {
                            hit = true;
                            closest = cacheCast.coefficient;
                            cast = cacheCast;
                            material = materials[triange.matId];
                        }
                    }
                    else
                    {
                        // Push stack
                        bvhStack[pos.X, pos.Y, ++stackIndex] = node.leftI;
                        bvhStack[pos.X, pos.Y, ++stackIndex] = node.rightI;
                    }
                }
            } while (stackIndex != -1) ;

            return hit;
        }

        /// <summary>
        /// Bounces a ray around a <see cref="Scene"/>.
        /// </summary>
        /// <param name="scene">The scene to bounce the ray in.</param>
        /// <param name="ray">The original ray to bounce.</param>
        /// <param name="randState">A integer used through out the shader to provide a random number.</param>
        /// <returns>The color of pixel from the original ray.</returns>
        private Float4 BounceRay(Scene scene, Ray ray, ref uint randState, Int2 pos)
        {
            Float4 color = Float4.Zero;
            Float4 cumAttenuation = Float4.One;

            // Bounce the ray around the scene iteratively
            for (int depth = 0; depth < scene.config.maxBounces; depth++)
            {
                if (GetHit(ray, out RayCast cast, out Material material, pos))
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
            // Position
            Int2 pos = ThreadIds.XY;

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
                color += BounceRay(scene, ray, ref randState, pos);
            }

            output[pos] = color / scene.config.samples;
        }

        public bool GetHitNoBVH(Ray ray, out RayCast cast, out Material material, Int2 pos)
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

            // Check each triangle
            for (int i = 0; i < geometry.Length; i++)
            {
                Triangle tri = geometry[i];
                if (Triangle.IsHit(tri, closest, ray, out cacheCast))
                {
                    hit = true;
                    closest = cacheCast.coefficient;
                    cast = cacheCast;
                    material = materials[tri.matId];
                }
            }

            return hit;
        }
    }
}
