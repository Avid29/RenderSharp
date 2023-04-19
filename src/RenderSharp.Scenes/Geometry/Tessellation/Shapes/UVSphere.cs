// Adam Dernis 2023

using RenderSharp.Scenes.Geometry.Meshes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace RenderSharp.Scenes.Geometry.Tessellation.Shapes;

/// <summary>
/// A class for uv spheres to be tessellated for rendering.
/// </summary>
public class UVSphere : TessellatedShape
{
    private int _segments;
    private int _rings;

    /// <summary>
    /// Initializes a new instance of the <see cref="UVSphere"/> class.
    /// </summary>
    public UVSphere()
    {
        Segments = 32;
        Rings = 16;
        Radius = 1;
    }

    /// <summary>
    /// Gets or sets the uv sphere segment count.
    /// </summary>
    public int Segments
    {
        get => _segments;
        set => ClampedSet(ref _segments, value, 3);
    }

    /// <summary>
    /// Gets or sets the uv sphere ring count.
    /// </summary>
    public int Rings
    {
        get => _rings;
        set => ClampedSet(ref _rings, value, 3);
    }

    /// <summary>
    /// Gets or sets the radius of the uv sphere.
    /// </summary>
    public float Radius { get; set; }
    
    /// <inheritdoc/>
    public override void ApplyTransformation(Transformation transformation)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public override Mesh ConvertToMesh()
    {
        Dictionary<Vector3, Vertex> vertices = new();
        List<Face> faces = new();

        int ring;
        float longitudinalStep = (360f / Rings).ToRadians();
        float latitudinalStep = (float)4 / Segments;

        void CreateRing(float upper, float lower)
        {
            var upperR = CircularEase(upper / Radius) * Radius;
            var lowerR = CircularEase(lower / Radius) * Radius;

            float x0 = 1; // Cos(0)
            float y0 = 0; // Sin(0)
            for (int segment = 1; segment <= Segments; segment++)
            {
                float phi = segment * longitudinalStep;
                float x1 = MathF.Cos(phi);
                float y1 = MathF.Sin(phi);

                Span<Vector3> vvs = stackalloc[]
                {
                    new Vector3(x0 * upperR, upper, y0 * upperR),
                    new Vector3(x0 * lowerR, lower, y0 * lowerR),
                    new Vector3(x1 * upperR, upper, y1 * upperR),
                    new Vector3(x1 * lowerR, lower, y1 * lowerR),
                };

                var vs = new Vertex[vvs.Length];

                for (int i = 0; i < vvs.Length; i++)
                {
                    if (!vertices.TryGetValue(vvs[i], out Vertex? vertex))
                    {
                        vertex = new Vertex(vvs[i])
                        {
                            // The normal of a vertex on a UV sphere is the vertex's position normalized
                            Normal = Vector3.Normalize(vvs[i]),
                        };

                        vertices.Add(vertex.Position, vertex);
                    }

                    vs[i] = vertex;
                }

                
                // If not first
                if (ring != Rings)
                    faces.Add(new Face(vs[0], vs[2], vs[3]));

                // If not last
                if (ring != 1)
                    faces.Add(new Face(vs[1], vs[0], vs[3]));
                
                x0 = x1;
                y0 = y1;
            }
        }

        // TODO: Circular ease on ring height
        float high = 1;
        for (ring = 1; ring <= Rings; ring++)
        {
            float low = high - latitudinalStep;
            CreateRing(high * Radius, low * Radius);
            high = low;
        }

        return new Mesh
        {
            Vertices = vertices.Values.ToList(),
            Faces = faces.ToList(),
        };
    }

    private float CircularEase(float t)
        => MathF.Sqrt(1 - t * t);

    private static void ClampedSet(ref int field, int value, int min)
    {
        if (value >= min)
            field = value;
    }
}
