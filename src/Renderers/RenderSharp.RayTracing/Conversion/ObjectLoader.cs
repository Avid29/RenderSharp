// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Scene.Geometry;
using RenderSharp.Scenes.Geometry;
using RenderSharp.Scenes.Geometry.Meshes;

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

    public void LoadObjects(List<GeometryObject> objects)
    {
        foreach (var obj in objects)
        {
            var mesh = obj.ConvertToMesh();
            LoadMesh(mesh);
        }

        AllocateBuffers();
    }

    public void LoadMesh(Mesh mesh)
    {
        foreach (var tri in mesh.Faces)
        {
            var triangle = new Triangle(tri.A.Position, tri.B.Position, tri.C.Position);
            _triangles.Add(triangle);
        }
    }

    private void AllocateBuffers()
    {
        GeometryBuffer = Device.AllocateReadOnlyBuffer(_triangles.ToArray());
    }
}
