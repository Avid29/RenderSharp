// Adam Dernis 2023

using ComputeSharp;

namespace RenderSharp.RayTracing.Models;

/// <summary>
/// A struct for pseudo-random numbers.
/// </summary>
public struct Rand
{
    private uint _state;
    
    /// <remarks>
    /// TODO: Replace with constructor
    /// Waiting on https://github.com/Sergio0694/ComputeSharp/issues/481
    /// </remarks>
    public static Rand Create(int x, int y, int sample)
    {
        Rand rand;
        rand._state = (uint)(x * 1973 + y * 9277 + sample * 266999) | 1;
        return rand;
    }

    /// <summary>
    /// Increments the rand state and returns.
    /// </summary>
    /// <returns>A pseudo-random unsigned integer.</returns>
    public uint Next()
    {
        // XorShift
        _state ^= _state << 13;
        _state ^= _state >> 17;
        _state ^= _state << 15;
        return _state;
    }
    
    /// <summary>
    /// Increments the rand state and returns.
    /// </summary>
    /// <returns>A pseudo-random floating point number.</returns>
    public float NextFloat() 
        => Next() * (1f / 4294967296f);
    
    /// <summary>
    /// Generates an in unit disk while incrementing the rand state.
    /// </summary>
    /// <returns>A pseudo-random in unit disk.</returns>
    public float3 NextInUnitDisk()
    {
        float3 p = new float3(NextFloat(), NextFloat(), 0) - new float3(1, 1, 0);
        return Hlsl.Normalize(p) / 2;
    }
}
