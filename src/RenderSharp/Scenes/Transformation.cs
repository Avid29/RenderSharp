﻿// Adam Dernis 2023

using System.Numerics;

namespace RenderSharp.Scenes;

public struct Transformation
{
    public Vector3 Translation { get; set; }

    public Quaternion Rotation { get; set; }
    
    public Vector3 Scale { get; set; }

    public Matrix4x4 ToTransformationMatrix()
    {
        var scale = Matrix4x4.CreateScale(Scale);
        var rotate = Matrix4x4.CreateFromQuaternion(Rotation);
        var translate = Matrix4x4.CreateTranslation(Translation);

        return scale * rotate * translate;
    }

    public static explicit operator Matrix4x4(Transformation transformation)
        => transformation.ToTransformationMatrix();
}