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
    public readonly partial struct PathTraceShader : IComputeShader
    {
        private readonly Scene scene;
        private readonly Int2 fullSize;
        private readonly Int2 offset;
        private readonly int bvhDepth;
        private readonly ReadOnlyBuffer<Triangle> geometry;
        private readonly ReadOnlyBuffer<Material> materials;
        private readonly ReadOnlyBuffer<BVHNode> bvhHeap;
        private readonly ReadWriteTexture3D<int> bvhStack;
        private readonly ReadWriteTexture3D<Float4> attenuationBuffer;
        private readonly ReadWriteTexture3D<Float4> colorStack;

        public bool GetHit(Ray ray, out RayCast cast, out Material material, Int3 pos)
        {
            return GetHitWithBVH(ray, out cast, out material, pos);
        }

        public bool GetHitWithBVH(Ray ray, out RayCast cast, out Material material, Int3 pos)
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
            bvhStack[pos.X, pos.Y, (pos.Z * bvhDepth) + stackIndex] = 0;

            do
            {
                // Pop the stack
                int nodeIndex = bvhStack[pos.X, pos.Y, (pos.Z * bvhDepth) + (stackIndex--)];
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
                        bvhStack[pos.X, pos.Y, (pos.Z * bvhDepth) + (++stackIndex)] = node.leftI;
                        bvhStack[pos.X, pos.Y, (pos.Z * bvhDepth) + (++stackIndex)] = node.rightI;
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
        private Ray BounceRay(Scene scene, Ray ray, ref uint randState, Int3 pos)
        {
            if (GetHit(ray, out RayCast cast, out Material material, pos))
            {
                Material.Emit(material, out Float4 emission);
                colorStack[pos] += emission * attenuationBuffer[pos];

                Material.Scatter(material, ray, cast, ref randState, out Float4 attenuation, out ray);
                attenuationBuffer[pos] *= attenuation;
            }
            else
            {
                // No object was hit
                // Therefore the sky was hit
                colorStack[pos] += attenuationBuffer[pos] * Sky.Color(scene.world.sky, ray);
                ray = Ray.Create();
            }

            return ray;
        }

        public void Execute()
        {
            // Position
            Int3 pos = ThreadIds.XYZ;
            int x = offset.X + ThreadIds.X;
            int y = offset.Y + ThreadIds.Y;
            int s = ThreadIds.Z;
            uint randState = (uint)(x * 1973 + y * 9277 + s * 26699) | 1;
            attenuationBuffer[pos] = Float4.One;

            float aspectRatio = (float)fullSize.X / fullSize.Y;
            FullCamera camera = FullCamera.Create(scene.camera, aspectRatio);
            float u = (x + RandUtils.RandomFloat(ref randState)) / fullSize.X;
            float v = 1 - ((y + RandUtils.RandomFloat(ref randState)) / fullSize.Y);
            Ray ray = FullCamera.CreateRay(camera, u, v, ref randState);

            for (int b = 0; b < scene.config.maxBounces; b++)
            {
                if (ray.direction.X == 0 && ray.direction.Y == 0 & ray.direction.Z == 0) break;
                ray = BounceRay(scene, ray, ref randState, pos);
            }
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
