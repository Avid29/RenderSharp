// Adam Dernis 2023

using RenderSharp.Scenes.Geometry.Meshes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace RenderSharp.Scenes.Geometry.Tessellation.Shapes;

public class UVSphere : TessellatedShape
{
    private int _segments;
    private int _rings;

    public UVSphere()
    {
        Segments = 32;
        Rings = 16;
        Radius = 1;
    }

    public int Segments
    {
        get => _segments;
        set => ClampedSet(ref _segments, value, 3);
    }

    public int Rings
    {
        get => _rings;
        set => ClampedSet(ref _rings, value, 3);
    }

    public float Radius { get; set; }

    public override Mesh ConvertToMesh()
    {
        Dictionary<Vector3, Vertex> vertices = new();
        List<Face> faces = new();

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

                faces.Add(new Face(vs[0], vs[1], vs[3]));
                faces.Add(new Face(vs[2], vs[0], vs[3]));
                
                x0 = x1;
                y0 = y1;
            }
        }

        // TODO: Circular ease on ring height
        float low = -1;
        for (int ring = 1; ring <= Rings; ring++)
        {
            float high = low + latitudinalStep;
            CreateRing(high * Radius, low * Radius);
            low = high;
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
