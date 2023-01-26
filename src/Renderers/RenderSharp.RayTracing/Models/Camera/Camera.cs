// Adam Dernis 2023

using RenderSharp.RayTracing.Models.Rays;
using RenderSharp.RayTracing.Utils;
using System.Numerics;

namespace RenderSharp.RayTracing.Models.Camera;

public struct Camera
{
    public Vector3 origin;
    public Vector3 u, v, n;
    public Vector3 horizontal, vertical;
    public Vector3 corner;

    /// <summary>
    /// Initializes a new instance of the <see cref="Camera"/> struct.
    /// </summary>
    /// <remarks>
    /// This constructor is designed to be called from the CPU unlike
    /// many of the models which are designed exclusively for shader execution.
    /// </remarks>
    public Camera(Vector3 origin, Vector3 lookAt, float fov, float focalLength, float aspectRatio)
    {
        float theta = FloatUtils.DegreesToRadians(fov);
        float h = MathF.Tan(theta / 2);

        float height = 2 * h;
        float width = height * aspectRatio;

        // TODO: Dutch angle support
        Vector3 cameraUp = Vector3.UnitY;

        this.origin = origin;
        this.n = Vector3.Normalize(origin - lookAt);
        this.u = Vector3.Normalize(Vector3.Cross(cameraUp, this.n));
        this.v = Vector3.Cross(this.n, this.u); // Implicitly normalized
        this.horizontal = width * this.u;
        this.vertical = height * this.v;

        var depth = this.n * focalLength;
        this.corner = this.origin - (this.horizontal / 2) - (this.vertical / 2) - depth;
    }

    /// <remarks>
    /// This should be a instance method. My disappoinment is immeasureable.
    /// TODO:https://github.com/Sergio0694/ComputeSharp/issues/479
    /// </remarks>
    public static Ray CreateRay(Camera c, float u, float v)
    {
        var origin = c.origin;
        var direction = c.corner + (u * c.horizontal) + (v * c.vertical) - origin;
        return Ray.Create(origin, direction);
    }
}
