﻿// Adam Dernis 2023

using RenderSharp.Scenes.Geometry.Meshes;
using System;
using System.Linq;
using System.Numerics;

namespace RenderSharp.Scenes.Geometry.Tessellation.Shapes;
public class Plane : TessellatedShape
{
    public Plane()
    {
        Size = 2;
    }

    public float Size { get; set; }

    public override Mesh ConvertToMesh()
    {
        Span<Vector3> vvs = stackalloc[]
        {
            new Vector3(-1, 0, -1),
            new Vector3(-1, 0, 1),
            new Vector3(1, 0, -1),
            new Vector3(1, 0, 1),
        };
        
        // Transform vertex vectors and make vertices
        var vs = new Vertex[vvs.Length];
        for (int i = 0; i < vvs.Length; i++)
        {
            ref var vertex = ref vvs[i];
            vertex *= Size / 2;

            // TODO: Vertex normals
            vs[i] = new Vertex(vertex);
        }

        var faces = new[]
        {
            new Face(vs[0], vs[1], vs[2]),
            new Face(vs[1], vs[3], vs[2]),
        };

        return new Mesh
        {
            Vertices = vs.ToList(),
            Faces = faces.ToList(),
        };
    }
}