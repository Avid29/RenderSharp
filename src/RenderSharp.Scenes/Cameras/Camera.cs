// Adam Dernis 2023

using RenderSharp.Scenes.Geometry;
using System;
using System.Numerics;

namespace RenderSharp.Scenes.Cameras;

/// <summary>
/// A camera for the common RenderSharp scene.
/// </summary>
public class Camera : Object
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Camera"/> class.
    /// </summary>
    /// <param name="origin">The camera origin.</param>
    /// <param name="rotation">The camera rotation.</param>
    /// <param name="fov">The camera field of view.</param>
    public Camera(Vector3 origin, Quaternion rotation, float fov)
    {
        Transformation = new Transformation
        {
            Translation = origin,
            Rotation = rotation,
        };

        Fov = fov;
    }

    /// <summary>
    /// Creates a new <see cref="Camera"/> from an origin, euler rotation, and a field of view.
    /// </summary>
    /// <param name="origin">The camera origin.</param>
    /// <param name="eulerXYZ">The euler rotation.</param>
    /// <param name="fov">The camera field of view.</param>
    /// <returns>A new <see cref="Camera"/>.</returns>
    public static Camera CreateFromEuler(Vector3 origin, Vector3 eulerXYZ, float fov)
    {
        var rotation = Quaternion.CreateFromYawPitchRoll(eulerXYZ.Y.ToRadians(), eulerXYZ.X.ToRadians(), eulerXYZ.Z.ToRadians());
        return new Camera(origin, rotation, fov);
    }

    /// <summary>
    /// Creates a new camera from an origin, look at coordinate, and a field of view.
    /// </summary>
    /// <param name="origin">The camera origin.</param>
    /// <param name="lookAt">The camera look at coordinate.</param>
    /// <param name="fov">The camera field of view.</param>
    /// <returns>A new <see cref="Camera"/>.</returns>
    public static Camera CreateFromLookAt(Vector3 origin, Vector3 lookAt, float fov)
    {
        // TODO: Specify camera up
        var direction = Vector3.Normalize(lookAt - origin);
        var axis = Vector3.Normalize(Vector3.Cross(Vector3.UnitZ, direction));
        var angle = MathF.Acos(Vector3.Dot(Vector3.UnitZ, direction));
        var rotation = Quaternion.CreateFromAxisAngle(axis, angle);

        return new Camera(origin, rotation, fov);
    }

    /// <summary>
    /// Gets or sets the field of view.
    /// </summary>
    public float Fov { get; set; }
}
