// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Scene.Geometry;
using RenderSharp.Scenes.Geometry;
using RenderSharp.Scenes.Geometry.Meshes;
using System.Numerics;

namespace RenderSharp.RayTracing.Conversion;

/// <summary>
/// A class for converting common RenderSharp objects into <see cref="RenderSharp.RayTracing"/> objects.
/// </summary>
public class ObjectLoader
{
    private readonly List<Triangle> _triangles;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectLoader"/> class.
    /// </summary>
    /// <param name="device"></param>
    public ObjectLoader(GraphicsDevice device)
    {
        Device = device;
        _triangles = new List<Triangle>();
    }

    /// <summary>
    /// Gets the device for buffer allocation.
    /// </summary>
    public GraphicsDevice Device { get; }

    /// <summary>
    /// Gets the generated geometry buffer.
    /// </summary>
    public ReadOnlyBuffer<Triangle>? GeometryBuffer { get; private set; }

    /// <summary>
    /// Gets the number of objects converted.
    /// </summary>
    public int ObjectCount { get; private set; }

    /// <summary>
    /// Loads a list of geometry objects in the geometry buffer.
    /// </summary>
    /// <param name="objects">The list of objects to load.</param>
    public void LoadObjects(List<GeometryObject> objects)
    {
        // Convert each object to a mesh and load the mesh with applied transformations.
        foreach (var obj in objects)
        {
            var mesh = obj.ConvertToMesh();
            LoadMesh(mesh, obj.Transformation);
            ObjectCount++;
        }

        AllocateBuffers();
    }

    private void LoadMesh(Mesh mesh, Transformation transformation)
    {
        // Load each face as a triangle
        foreach (var tri in mesh.Faces)
        {
            // TODO: Polygon triangulation

            var a = tri.A.Position;
            var b = tri.B.Position;
            var c = tri.C.Position;

            a = Vector3.Transform(a, (Matrix4x4)transformation);
            b = Vector3.Transform(b, (Matrix4x4)transformation);
            c = Vector3.Transform(c, (Matrix4x4)transformation);

            // Track which the triangle's object id.
            int objectId = ObjectCount;
            var triangle = new Triangle(a, b, c, objectId);
            _triangles.Add(triangle);
        }
    }

    private void AllocateBuffers()
    {
        GeometryBuffer = Device.AllocateReadOnlyBuffer(_triangles.ToArray());
    }
}
