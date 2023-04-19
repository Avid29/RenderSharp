// Adam Dernis 2023

using System.Numerics;

namespace RenderSharp.Scenes.Geometry;

/// <summary>
/// A struct for object transformation data.
/// </summary>
public struct Transformation
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Transformation"/> struct.
    /// </summary>
    public Transformation()
    {
        Translation = Vector3.Zero;
        Rotation = Quaternion.Identity;
        Scale = Vector3.One;
    }

    /// <summary>
    /// Creates a new <see cref="Transformation"/> from a <see cref="Vector3"/> translation.
    /// </summary>
    /// <param name="translation">The translation component.</param>
    /// <returns></returns>
    public static Transformation CreateFromTranslation(Vector3 translation)
    {
        return new Transformation { Translation = translation };
    }

    /// <summary>
    /// Gets or sets the object translation.
    /// </summary>
    public Vector3 Translation { get; set; }

    /// <summary>
    /// Gets or sets the object rotation.
    /// </summary>
    public Quaternion Rotation { get; set; }

    /// <summary>
    /// Gets or sets the object scaling.
    /// </summary>
    public Vector3 Scale { get; set; }

    /// <summary>
    /// Converts the <see cref="Transformation"/> to a <see cref="Matrix4x4"/> transformation.
    /// </summary>
    /// <returns></returns>
    public Matrix4x4 ToTransformationMatrix()
    {
        var scale = Matrix4x4.CreateScale(Scale);
        var rotate = Matrix4x4.CreateFromQuaternion(Rotation);
        var translate = Matrix4x4.CreateTranslation(Translation);
        return scale * rotate * translate;
    }

    /// <summary>
    /// Explicitly casts a <see cref="Transformation"/> to a <see cref="Matrix4x4"/> transformation.
    /// </summary>
    /// <param name="transformation">The <see cref="Transformation"/> to cast.</param>
    public static explicit operator Matrix4x4(Transformation transformation)
        => transformation.ToTransformationMatrix();
}
