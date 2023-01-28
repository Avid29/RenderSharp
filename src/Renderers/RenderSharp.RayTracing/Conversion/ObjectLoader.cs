// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Scene.Geometry;
using RenderSharp.Scenes.Geometry;
using RenderSharp.Scenes.Geometry.Meshes;
using System.Numerics;

namespace RenderSharp.RayTracing.Conversion;

public class ObjectLoader
{
    private List<Triangle> _triangles;

    public ObjectLoader(GraphicsDevice device)
    {
        Device = device;
        _triangles = new List<Triangle>();
    }

    public GraphicsDevice Device { get; set; }

    public ReadOnlyBuffer<Triangle>? GeometryBuffer { get; private set; }

    public int ObjectCount { get; set; }

    public void LoadObjects(List<GeometryObject> objects)
    {
        foreach (var obj in objects)
        {
            var mesh = obj.ConvertToMesh();
            LoadMesh(mesh, obj.Transformation);
            ObjectCount++;
        }

        AllocateBuffers();
    }

    public void LoadMesh(Mesh mesh, Transformation transformation)
    {
        foreach (var tri in mesh.Faces)
        {
            var a = tri.A.Position;
            var b = tri.B.Position;
            var c = tri.C.Position;

            a = Vector3.Transform(a, (Matrix4x4)transformation);
            b = Vector3.Transform(b, (Matrix4x4)transformation);
            c = Vector3.Transform(c, (Matrix4x4)transformation);

            var triangle = new Triangle(a, b, c, ObjectCount);
            _triangles.Add(triangle);
        }
    }

    private void AllocateBuffers()
    {
        GeometryBuffer = Device.AllocateReadOnlyBuffer(_triangles.ToArray());
    }
}
