using System.Numerics;

namespace RenderSharp.RayTracing.Utils
{
    public static class RandUtils
    {
        public static uint XorShift(ref uint state)
        {
            state ^= state << 13;
            state ^= state >> 17;
            state ^= state << 15;
            return state;
        }

        public static float RandomFloat(ref uint state)
        {
            return XorShift(ref state) * (1f / 4294967296f);
        }

        public static float3 RandomInUnitDisk(ref uint state)
        {
            float3 p;
            do
            {
                p = 2f * new float3(RandomFloat(ref state), RandomFloat(ref state), 0) - new float3(1, 1, 0);
            } while (Vector3.Dot(p, p) >= 1f);
            return p;
        }

        public static float3 RandomInUnitSphere(ref uint state)
        {
            float3 ret;
            do
            {
                ret = 2f * new float3(RandomFloat(ref state), RandomFloat(ref state), RandomFloat(ref state)) - float3.One;
            } while (FloatUtils.LengthSquared(ret) >= 1f);
            return ret;
        }
    }
}
