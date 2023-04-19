// Adam Dernis 2023

using ComputeSharp;

namespace RenderSharp.RayTracing.RayCasts;

/// <summary>
/// Primary ray collision information tracked for shading.
/// </summary>
public struct GeometryCollision
{
    /// <summary>
    /// The id of the geometry hit.
    /// </summary>
    public int geoId;

    /// <summary>
    /// The intersection position in world space.
    /// </summary>
    public float3 position;

    /// <summary>
    /// The flat geometry normal.
    /// </summary>
    public float3 normal;

    /// <summary>
    /// The smoothed geometry normal.
    /// </summary>
    public float3 smoothNormal;

    /// <summary>
    /// The barycentric coordinate of the intersection.
    /// </summary>
    public float2 uv;

    /// <summary>
    /// The distance of the collision.
    /// </summary>
    public float distance;

    /// <summary>
    /// Whether or not the collision hit the backface.
    /// </summary>
    public Bool isBackFace;

    /// <summary>
    /// The id of the geometry's material.
    /// </summary>
    public int matId;

    /// <summary>
    /// The id of the geometry's object.
    /// </summary>
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
    
    /// <summary>
    /// Creates a default geometry collision.
    /// </summary>
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
