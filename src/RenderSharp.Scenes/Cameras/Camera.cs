// Adam Dernis 2023

using RenderSharp.Scenes.Geometry;
using System.Numerics;

namespace RenderSharp.Scenes.Cameras;

public class Camera : Object
{
    public Camera(Vector3 origin, Quaternion rotation, float fov)
    {
        Transformation = new Transformation
        {
            Translation = origin,
            Rotation = rotation,
        };

        Fov = fov;
    }

    public static Camera CreateFromEuler(Vector3 origin, Vector3 eulerXYZ, float fov)
    {
        var rotation = Quaternion.CreateFromYawPitchRoll(eulerXYZ.Y.ToRadians(), eulerXYZ.X.ToRadians(), eulerXYZ.Z.ToRadians());
        return new Camera(origin, rotation, fov);
    }

    public static Camera CreateFromLookAt(Vector3 origin, Vector3 lookAt, float fov)
    {
        // TODO: Specify camera up
        var direction = Vector3.Normalize(lookAt - origin);
        var axis = Vector3.Normalize(Vector3.Cross(Vector3.UnitZ, direction));
        var angle = MathF.Acos(Vector3.Dot(Vector3.UnitZ, direction));
        var rotation = Quaternion.CreateFromAxisAngle(axis, angle);

        return new Camera(origin, rotation, fov);
    }

    public float Fov { get; set; }
}
