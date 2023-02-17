// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Models.Geometry;
using RenderSharp.RayTracing.Models.Lighting;
using RenderSharp.Scenes.Geometry;
using RenderSharp.Scenes.Geometry.Meshes;
using RenderSharp.Scenes.Lights;
using System.Numerics;
using CommonScene = RenderSharp.Scenes.Scene;

namespace RenderSharp.RayTracing.Setup;

/// <summary>
/// A class for converting common RenderSharp objects into <see cref="RenderSharp.RayTracing"/> objects.
/// </summary>
public class ObjectLoader
{
    private readonly List<Triangle> _triangles;
    private readonly List<Light> _lights;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectLoader"/> class.
    /// </summary>
    /// <param name="device"></param>
    public ObjectLoader(GraphicsDevice device)
    {
        Device = device;
        _triangles = new List<Triangle>();
        _lights = new List<Light>();
    }

    /// <summary>
    /// Gets the device for buffer allocation.
    /// </summary>
    public GraphicsDevice Device { get; }

    /// <summary>
    /// Gets the generated geometry buffer.
    /// </summary>
    public ReadOnlyBuffer<Triangle>? GeometryBuffer { get; private set; }

    public ReadOnlyBuffer<Light>? LightsBuffer { get; private set; }

    /// <summary>
    /// Gets the number of objects converted.
    /// </summary>
    public int ObjectCount { get; private set; }
    
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
            ObjectCount++;
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
        => new(Device, _triangles);

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
            // TODO: Assign Material ID
            int objectId = ObjectCount;
            var triangle = new Triangle(a, b, c, 0, objectId);
            _triangles.Add(triangle);
        }
    }

    private void AllocateBuffers()
    {
        GeometryBuffer = Device.AllocateReadOnlyBuffer(_triangles.ToArray());
        LightsBuffer = Device.AllocateReadOnlyBuffer(_lights.ToArray());
    }
}
