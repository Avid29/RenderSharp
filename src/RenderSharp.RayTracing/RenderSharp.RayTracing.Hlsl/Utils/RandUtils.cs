using ComputeSharp;
using System.Numerics;

namespace RenderSharp.RayTracing.HLSL.Utils
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

        public static Float3 RandomInUnitDisk(ref uint state)
        {
            Float3 p;
            do
            {
                p = 2f * new Float3(RandomFloat(ref state), RandomFloat(ref state), 0) - new Float3(1, 1, 0);
            } while (Vector3.Dot(p, p) >= 1f);
            return p;
        }

        public static Float3 RandomInUnitSphere(ref uint state)
        {
            Float3 ret;
            do
            {
                ret = 2f * new Float3(RandomFloat(ref state), RandomFloat(ref state), RandomFloat(ref state)) - Float3.One;
            } while (FloatUtils.LengthSquared(ret) >= 1f);
            return ret;
        }
    }
}
