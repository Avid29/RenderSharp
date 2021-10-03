using ComputeSharp;
using RenderSharp.RayTracing.HLSL.Rays;
using RenderSharp.RayTracing.HLSL.Utils;

namespace RenderSharp.RayTracing.HLSL.Materials
{
    public struct Material
    {
        public Float4 albedo;
        public Float4 emission;

        public static Material Create(Float4 albedo, Float4 emission)
        {
            Material material;
            material.albedo = albedo;
            material.emission = emission;
            return material;
        }

        public static void Scatter(Material material, Ray ray, RayCast cast, ref uint randState, out Float4 attenuation, out Ray scatter)
        {
            Float3 target = cast.origin + cast.normal + RandUtils.RandomInUnitSphere(ref randState);
            attenuation = material.albedo;
            scatter = Ray.Create(cast.origin, target - cast.origin);
        }

        public static void Emit(Material material, out Float4 emission)
        {
            emission = material.emission;
        }
    }
}
