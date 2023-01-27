// Adam Dernis 2023

using RenderSharp.Scenes.Geometry;
using System.Numerics;

namespace RenderSharp.Scenes.Cameras;

public class Camera : Object
{
    public Camera(Vector3 origin, Vector3 eulerXYZ, float fov)
    {
        var rotation = Quaternion.CreateFromYawPitchRoll(eulerXYZ.Y.ToRadians(), eulerXYZ.X.ToRadians(), eulerXYZ.Z.ToRadians());

        Transformation = new Transformation
        {
            Translation = origin,
            Rotation = rotation,
        };

        Fov = fov;
    }

    public float Fov { get; set; }
}
