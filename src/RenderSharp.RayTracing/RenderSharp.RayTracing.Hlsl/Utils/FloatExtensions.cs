using ComputeSharp;

namespace RenderSharp.RayTracing.HLSL.Utils
{
    public static class FloatUtils
    {
        public static float LengthSquared(Float2 v2)
        {
            return v2.X * v2.X + v2.Y * v2.Y;
        }

        public static float LengthSquared(Float3 v3)
        {
            return v3.X * v3.X + v3.Y * v3.Y + v3.Z * v3.Z;
        } 
    }
}
