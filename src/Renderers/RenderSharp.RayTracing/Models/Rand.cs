// Adam Dernis 2023

using ComputeSharp;

namespace RenderSharp.RayTracing.Models;

public struct Rand
{
    private uint _state;

    public static Rand Create(int x, int y, int sample)
    {
        Rand rand;
        rand._state = (uint)(x * 1973 + y * 9277 + sample * 266999) | 1;
        return rand;
    }

    public uint Next()
    {
        // XorShift
        _state ^= _state << 13;
        _state ^= _state >> 17;
        _state ^= _state << 15;
        return _state;
    }

    public float NextFloat() 
        => Next() * (1f / 4294967296f);

    public float3 NextInUnitDisk()
    {
        float3 p = new float3(NextFloat(), NextFloat(), 0) - new float3(1, 1, 0);
        return Hlsl.Normalize(p) / 2;
    }
}
