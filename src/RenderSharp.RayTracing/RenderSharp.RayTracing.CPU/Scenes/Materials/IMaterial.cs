using RenderSharp.RayTracing.CPU.Scenes.Rays;
using System.Numerics;

namespace RenderSharp.RayTracing.CPU.Scenes.Materials
{
    public interface IMaterial
    {
        public void Emit(out Vector4 emission);

        public void Scatter(Ray ray, RayCast cast, ref uint randState, out Vector4 attenuation, out Ray scatter);
    }
}
