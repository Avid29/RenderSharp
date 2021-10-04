using RenderSharp.RayTracing.CPU.Rays;
using RenderSharp.RayTracing.CPU.Utils;
using System.Numerics;

namespace RenderSharp.RayTracing.CPU.Materials
{
    public struct SuperMaterial : IMaterial
    {
        public SuperMaterial(Vector4 albedo, float metallic, float roughness, Vector4 emission)
        {
            Albedo = albedo;
            Metallic = metallic;
            Roughness = roughness;
            Emission = emission;
        }

        public Vector4 Albedo { get; }

        public float Metallic { get; }

        public float Roughness { get; }

        public Vector4 Emission { get; }

        public void Scatter(Ray ray, RayCast cast, ref uint randState, out Vector4 attenuation, out Ray scatter)
        {
            bool reflect;
            if (Metallic == 1) reflect = true;
            else if (Metallic == 0) reflect = false;
            else reflect = RandUtils.RandomFloat(ref randState) < Metallic;

            Vector3 target;
            if (reflect) target = Vector3.Reflect(Vector3.Normalize(ray.Direction), cast.Normal); // Render as metal.
            else target = cast.Origin + cast.Normal; // Render as diffuse

            // Apply roughness
            target += Roughness * RandUtils.RandomInUnitSphere(ref randState);

            attenuation = Albedo;
            scatter = new Ray(cast.Origin, target - cast.Origin);
        }

        public void Emit(out Vector4 emission)
        {
            emission = Emission;
        }
    }
}
