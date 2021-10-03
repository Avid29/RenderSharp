using ComputeSharp;

namespace RenderSharp.RayTracing.HLSL.Rays
{
    public struct RayCast
    {
        public Float3 origin;
        public Float3 normal;
        public Float2 uv;
        public float coefficient;
    }
}
