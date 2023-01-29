﻿// Adam Dernis 2023

namespace RenderSharp.RayTracing.Scene.Rays;

/// <summary>
/// Primary ray collision information tracked for shading.
/// </summary>
public struct RayCast
{
    public float3 position;
    public float3 normal;
    public float distance;
    public int triId;
    public int matId;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="RayCast"/> struct.
    /// </summary>
    public RayCast(float3 position, float3 normal, float distance)
    {
        this.position = position;
        this.normal = normal;
        this.distance = distance;
        this.triId = -1;
        this.matId = -1;
    }
    
    /// <remarks>
    /// TODO: Replace with constructor
    /// Waiting on https://github.com/Sergio0694/ComputeSharp/issues/481
    /// </remarks>
    public static RayCast Create(float3 position, float3 normal, float distance)
    {
        RayCast cast;
        cast.position = position;
        cast.normal = normal;
        cast.distance = distance;
        cast.triId = -1;
        cast.matId = -1;
        return cast;
    }
}
