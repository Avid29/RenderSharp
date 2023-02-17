﻿// Adam Dernis 2023

namespace RenderSharp.RayTracing.Models.Rays;

/// <summary>
/// Primary ray collision information tracked for shading.
/// </summary>
public struct GeometryCollision
{
    public int geoId;
    public float3 position;
    public float3 normal;
    public float distance;
    public int matId;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="GeometryCollision"/> struct.
    /// </summary>
    public GeometryCollision(float3 position, float3 normal, float distance)
    {
        this.position = position;
        this.normal = normal;
        this.distance = distance;
        this.geoId = -1;
        this.matId = -1;
    }
    
    /// <remarks>
    /// TODO: Replace with constructor
    /// Waiting on https://github.com/Sergio0694/ComputeSharp/issues/481
    /// </remarks>
    public static GeometryCollision Create(float3 position, float3 normal, float distance)
    {
        GeometryCollision cast;
        cast.position = position;
        cast.normal = normal;
        cast.distance = distance;
        cast.geoId = -1;
        cast.matId = -1;
        return cast;
    }
}
