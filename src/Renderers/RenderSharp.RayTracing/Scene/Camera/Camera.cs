﻿// Adam Dernis 2023

using RenderSharp.RayTracing.Scene.Rays;
using RenderSharp.RayTracing.Utils;
using RenderSharp.Scenes.Geometry;
using System.Numerics;

namespace RenderSharp.RayTracing.Scene.Camera;

/// <summary>
/// A camera for creating rays.
/// </summary>
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
    public Camera(Transformation transformation, float fov, float aspectRatio)
    {
        float theta = FloatUtils.DegreesToRadians(fov);
        float h = MathF.Tan(theta / 2);

        float height = 2 * h;
        float width = height * aspectRatio;

        // Camera faces negative -Vector3.UnitZ by default
        this.origin = transformation.Translation;
        this.u = Vector3.Transform(Vector3.UnitX, transformation.Rotation);
        this.v = Vector3.Transform(Vector3.UnitY, transformation.Rotation);
        this.n = Vector3.Transform(-Vector3.UnitZ, transformation.Rotation);
        this.horizontal = width * this.u;
        this.vertical = height * this.v;

        // TODO: Focal length
        var depth = this.n;
        this.corner = this.origin - (this.horizontal / 2) - (this.vertical / 2) - depth;
    }

    /// <summary>
    /// Creates a ray from the camera going from the origin through the uv coordinate on the camera projected view.
    /// </summary>
    /// <remarks>
    /// TODO: Convert to an instance method.
    /// Waiting on https://github.com/Sergio0694/ComputeSharp/issues/479
    /// </remarks>
    public static Ray CreateRay(Camera c, float u, float v)
    {
        var origin = c.origin;
        var direction = c.corner + (u * c.horizontal) + (v * c.vertical) - origin;
        return Ray.Create(origin, direction);
    }
}