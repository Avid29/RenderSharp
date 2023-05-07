// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Models.Geometry;
using RenderSharp.RayTracing.RayCasts;
using RenderSharp.RayTracing.Shaders.Pipeline.Collision.Interfaces;

namespace RenderSharp.RayTracing.Shaders.Pipeline.Collision;

/// <summary>
/// An <see cref="ICollisionShader"/> without BVH tree optimization.
/// </summary>
[AutoConstructor]
[EmbeddedBytecode(DispatchAxis.X)]
public readonly partial struct GeometryCollisionShader : ICollisionShader
{
    private readonly ReadOnlyBuffer<Vertex> vertexBuffer;
    private readonly ReadOnlyBuffer<Triangle> geometryBuffer;
    private readonly ReadWriteBuffer<Ray> rayBuffer;
    private readonly ReadWriteBuffer<GeometryCollision> rayCastBuffer;
    private readonly int collisionMode;

    private bool IsClosestHit(Triangle tri, Ray ray, float maxClip, out GeometryCollision cast)
    {
        var a = vertexBuffer[tri.a];
        var b = vertexBuffer[tri.b];
        var c = vertexBuffer[tri.c];

        // Set default cast values
        cast = GeometryCollision.Create();

        // Find the triangle's normal direction
        var normal = Hlsl.Normalize(Hlsl.Cross(b.position - a.position, c.position - a.position));

        // Find the length required for the ray to collide with the triangle's plane
        var dn = Hlsl.Dot(normal, ray.direction);
        float t = (Hlsl.Dot(normal, a.position) - Hlsl.Dot(normal, ray.origin)) / dn;

        // Check for backface collision
        bool isBackFace = dn > 0;   

        // Ensure the collision is in the positive direction, and not outside the clipped range
        if (t < 0.0001f || t > maxClip)
            return false;

        // Find the collision point on the plane
        var q = Ray.PointAt(ray, t);

        // Calculate barycentric coordinates
        float u = Hlsl.Dot(Hlsl.Cross(b.position - a.position, q - a.position), normal);
        float v = Hlsl.Dot(Hlsl.Cross(c.position - b.position, q - b.position), normal);
        float w = Hlsl.Dot(Hlsl.Cross(a.position - c.position, q - c.position), normal);

        // Ensure the ray collides with the triangle's plane within the bounds of the triangle's face
        if (u < 0 || v < 0 || w < 0)
            return false;

        // Calculate smooth normal from vertex normals
        var smoothNormal = Hlsl.Normalize(c.normal * u + a.normal * v + b.normal * w);

        // Revert smooth normal to normal if vertex normals are 0
        if (Hlsl.Length(smoothNormal) == 0)
            smoothNormal = normal;

        cast = GeometryCollision.Create(q, normal, smoothNormal, new float2(u, v), t, isBackFace);
        return true;
    }

    /// <inheritdoc/>
    public void Execute()
    {
        int index = ThreadIds.X;

        Ray ray = rayBuffer[index];
        if (Hlsl.Length(ray.direction) == 0)
            return;

        var rayCast = GeometryCollision.Create();

        // Track the nearest scene collision
        float distance = float.MaxValue;

        // Check for collision with every triangle in the geometry buffer
        for (int i = 0; i < geometryBuffer.Length; i++)
        {
            var tri = geometryBuffer[i];

            if (!IsClosestHit(tri, ray, distance, out var cast))
                continue;

            distance = cast.distance;
            cast.geoId = i;
            cast.matId = tri.matId;
            cast.objId = tri.objId;
            rayCast = cast;

            // Return first collision when in CollisionMode.Any
            // TODO: Check if collision is behind light source
            if (collisionMode == 1)
                break;
        }

        // Store the ray cast in the ray cast buffer
        rayCastBuffer[index] = rayCast;
    }
}
