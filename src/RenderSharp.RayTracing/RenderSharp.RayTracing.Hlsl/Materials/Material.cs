using ComputeSharp;
using RenderSharp.RayTracing.HLSL.Rays;
using RenderSharp.RayTracing.HLSL.Textures;
using RenderSharp.RayTracing.HLSL.Utils;

namespace RenderSharp.RayTracing.HLSL.Materials
{
    public struct Material
    {
        public Texture albedo;
        public Float4 emission;
        public float roughness;
        public float metallic;

        public static Material Create(Texture albedo, Float4 emission, float roughness, float metallic)
        {
            Material material;
            material.albedo = albedo;
            material.emission = emission;
            material.roughness = roughness;
            material.metallic = metallic;
            return material;
        }

        public static void Scatter(Material material, Ray ray, RayCast cast, ref uint randState, out Float4 attenuation, out Ray scatter)
        {
            bool reflect;
            if (material.metallic == 1) reflect = true;
            else if (material.metallic == 0) reflect = false;
            else reflect = RandUtils.RandomFloat(ref randState) < material.metallic;

            Float3 target;
            if (reflect) target = Hlsl.Reflect(Hlsl.Normalize(ray.direction), cast.normal); // Render as metal.
            else target = cast.origin + cast.normal; // Render as diffuse

            // Apply roughness
            target += material.roughness * RandUtils.RandomInUnitSphere(ref randState);

            attenuation = Texture.Value(material.albedo, cast.uv);
            scatter = Ray.Create(cast.origin, target - cast.origin);
        }

        public static void Emit(Material material, out Float4 emission)
        {
            emission = material.emission;
        }
    }
}
