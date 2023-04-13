// Adam Dernis 2023

using ComputeSharp;

namespace RenderSharp.RayTracing.Models.Rays;

/// <summary>
/// Primary ray collision information tracked for shading.
/// </summary>
public struct GeometryCollision
{
    public int geoId;
    public float3 position;
    public float3 normal;
    public float3 smoothNormal;
    public float2 uv;
    public float distance;
    public Bool isBackFace;
    public int matId;
    public int objId;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="GeometryCollision"/> struct.
    /// </summary>
    public GeometryCollision(float3 position, float3 normal, float3 smoothNormal, float2 uv, float distance, bool isBackFace)
    {
        this.position = position;
        this.normal = normal;
        this.distance = distance;
        this.smoothNormal = smoothNormal;
        this.isBackFace = isBackFace;
        this.geoId = -1;
        this.matId = -1;
        this.objId = -1;
    }
    
    public static GeometryCollision Create()
        => Create(0, 0, 0, 0, 0, false);

    /// <remarks>
    /// TODO: Replace with constructor
    /// Waiting on https://github.com/Sergio0694/ComputeSharp/issues/481
    /// </remarks>
    public static GeometryCollision Create(float3 position, float3 normal, float3 smoothNormal, float2 uv, float distance, bool isBackFace)
    {
        GeometryCollision cast;
        cast.position = position;
        cast.normal = normal;
        cast.smoothNormal = smoothNormal;
        cast.uv = uv;
        cast.distance = distance;
        cast.isBackFace = isBackFace;
        cast.geoId = -1;
        cast.matId = -1;
        cast.objId = -1;
        return cast;
    }
}
