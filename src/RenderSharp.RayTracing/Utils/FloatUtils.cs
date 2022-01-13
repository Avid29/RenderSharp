using System;

namespace RenderSharp.RayTracing.Utils
{
    public static class FloatUtils
    {
        public static float LengthSquared(float2 v2)
        {
            return v2.X * v2.X + v2.Y * v2.Y;
        }

        public static float LengthSquared(float3 v3)
        {
            return v3.X * v3.X + v3.Y * v3.Y + v3.Z * v3.Z;
        }

        public static float DegreesToRadians(float radians)
        {
#if NET5_0_OR_GREATER
            return MathF.PI / 180f * radians;
#elif NETSTANDARD2_0_OR_GREATER
            return (float)Math.PI / 180f * radians;
#endif
        }
    }
}
