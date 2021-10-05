using RenderSharp.RayTracing.CPU.Scenes.Rays;
using RenderSharp.RayTracing.CPU.Utils;
using System.Numerics;

namespace RenderSharp.RayTracing.CPU.Scenes.Materials
{
    public struct DiffuseMaterial : IMaterial
    {
        public DiffuseMaterial(Vector4 albedo, float roughness)
        {
            Albedo = albedo;
            Roughness = roughness;
        }

        public Vector4 Albedo { get; }

        public float Roughness { get; }

        public void Scatter(Ray ray, RayCast cast, ref uint randState, out Vector4 attenuation, out Ray scatter)
        {
            Vector3 target = cast.Origin + cast.Normal;

            // Apply roughness
            target += Roughness * RandUtils.RandomInUnitSphere(ref randState);

            attenuation = Albedo;
            scatter = new Ray(cast.Origin, target - cast.Origin);
        }

        public void Emit(out Vector4 emission)
        {
            emission = Vector4.Zero;
        }
    }
}
