// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Models.Geometry;
using RenderSharp.RayTracing.Models.Lighting;
using RenderSharp.Scenes.Geometry;
using RenderSharp.Scenes.Geometry.Meshes;
using RenderSharp.Scenes.Lights;
using System.Numerics;
using CommonScene = RenderSharp.Scenes.Scene;
using CommonVertex = RenderSharp.Scenes.Geometry.Meshes.Vertex;
using Vertex = RenderSharp.RayTracing.Models.Geometry.Vertex;

namespace RenderSharp.RayTracing.Setup;

/// <summary>
/// A class for converting common RenderSharp objects into <see cref="RenderSharp.RayTracing"/> objects.
/// </summary>
public class ObjectLoader
{
    private readonly List<ObjectSpace> _objects;
    private readonly List<Vertex> _vertices;
    private readonly List<Triangle> _triangles;
    private readonly List<Light> _lights;
    private readonly Dictionary<CommonVertex, int> _vertexIndexDictionary;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectLoader"/> class.
    /// </summary>
    /// <param name="device"></param>
    public ObjectLoader(GraphicsDevice device)
    {
        Device = device;
        _objects = new List<ObjectSpace>();
        _vertices = new List<Vertex>();
        _triangles = new List<Triangle>();
        _lights = new List<Light>();
        _vertexIndexDictionary= new  Dictionary<CommonVertex, int>();
    }

    /// <summary>
    /// Gets the device for buffer allocation.
    /// </summary>
    public GraphicsDevice Device { get; }

    /// <summary>
    /// Gets the generated object buffer.
    /// </summary>
    public ReadOnlyBuffer<ObjectSpace>? ObjectBuffer { get; private set; }

    /// <summary>
    /// Gets the generated vertex buffer.
    /// </summary>
    public ReadOnlyBuffer<Vertex>? VertexBuffer { get; private set; }

    /// <summary>
    /// Gets the generated geometry buffer.
    /// </summary>
    public ReadOnlyBuffer<Triangle>? GeometryBuffer { get; private set; }
    
    /// <summary>
    /// Gets the generated light buffer.
    /// </summary>
    public ReadOnlyBuffer<Light>? LightsBuffer { get; private set; }
    
    /// <summary>
    /// Loads a scene into the appropriate buffers.
    /// </summary>
    public void LoadScene(CommonScene scene)
    {
        LoadObjects(scene.Geometry);
        LoadLights(scene.Lights);

        AllocateBuffers();
    }

    private void LoadObjects(List<GeometryObject> objects)
    {
        // Convert each object to a mesh and load the mesh with applied transformations.
        foreach (var obj in objects)
        {
            var mesh = obj.ConvertToMesh();
            LoadMesh(mesh, obj.Transformation);
            Matrix4x4.Invert(Matrix4x4.CreateTranslation(-1.25f, 0, 4f), out var m);
            _objects.Add(new ObjectSpace
            {
                //inverseTransformation = obj.Transformation.ToTransformationMatrix().ToFloat4x4(),
                inverseTransformation = m.ToFloat4x4(),
            });
        }
    }

    private void LoadLights(List<LightSource> lightSources)
    {
        // TODO: Handle non-point light sources
        foreach (var lightSource in lightSources)
        {
            var position = lightSource.Transformation.Translation;
            var color = lightSource.Color;
            var power = lightSource.Power;
            
            if (lightSource is PointLight pointLight)
            {
                var radius = pointLight.Radius;
                var light = new Light(position, color, power, radius);
                _lights.Add(light);
            }
        }
    }

    /// <summary>
    /// Gets a <see cref="BVHBuilder"/> for the loaded geometry.
    /// </summary>
    public BVHBuilder GetBVHBuilder()
        => new(Device, _vertices, _triangles);

    private void LoadMesh(Mesh mesh, Transformation transformation)
    {
        // Load each face as a triangle
        foreach (var tri in mesh.Faces)
        {
            // TODO: Polygon triangulation
            var a = LoadVertex(tri.A, transformation);
            var b = LoadVertex(tri.B, transformation);
            var c = LoadVertex(tri.C, transformation);

            // Track the id of the object the triangle belongs to.
            int objectId = _objects.Count;
            // TODO: Properly assign Material ID
            int matId = objectId;
            var triangle = new Triangle(a, b, c, matId, objectId);
            _triangles.Add(triangle);
        }
    }

    private int LoadVertex(CommonVertex vertex, Transformation transformation)
    {
        if (!_vertexIndexDictionary.ContainsKey(vertex))
        {
            var pos = Vector3.Transform(vertex.Position, (Matrix4x4)transformation);
            var v = new Vertex(pos, vertex.Normal);
            _vertexIndexDictionary.Add(vertex, _vertices.Count);
            _vertices.Add(v);
        }
        
        return _vertexIndexDictionary[vertex];
    }

    private void AllocateBuffers()
    {
        ObjectBuffer = Device.AllocateReadOnlyBuffer(_objects.ToArray());
        VertexBuffer = Device.AllocateReadOnlyBuffer(_vertices.ToArray());
        GeometryBuffer = Device.AllocateReadOnlyBuffer(_triangles.ToArray());
        LightsBuffer = Device.AllocateReadOnlyBuffer(_lights.ToArray());
    }
}
