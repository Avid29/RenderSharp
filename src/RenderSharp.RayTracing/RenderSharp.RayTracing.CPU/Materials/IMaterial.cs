using RenderSharp.RayTracing.CPU.Rays;
using System.Numerics;

namespace RenderSharp.RayTracing.CPU.Materials
{
    public interface IMaterial
    {
        public void Emit(out Vector4 emission);

        public void Scatter(Ray ray, RayCast cast, ref uint randState, out Vector4 attenuation, out Ray scatter);
    }
}
