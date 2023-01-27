// Adam Dernis 2023

using RenderSharp.Scenes.Geometry.Meshes;
using System.Linq;
using System.Numerics;

namespace RenderSharp.Scenes.Geometry.Tessellation.Shapes;

public class TessellatedCube : TessellatedShape
{
    public TessellatedCube()
    {
        Center = Vector3.Zero;
        Scale = Vector3.One;
    }

    public override Mesh ConvertToMesh()
    {
        Transformation transform = new Transformation()
        {
            Translation = Center,
            Scale = Scale,
        };

        // Define vertex vectors
        var vvs = new[]
        {
            new Vector3(-1, -1, -1),
            new Vector3(1, -1, -1),
            new Vector3(-1, 1, -1),
            new Vector3(-1, -1, 1),
            new Vector3(1, 1, -1),
            new Vector3(-1, 1, 1),
            new Vector3(1, -1, 1),
            new Vector3(1, 1, 1),
        };

        // Transform vertex vectors and make vertices
        var vs = new Vertex[vvs.Length];
        for (int i = 0; i < vvs.Length; i++)
        {
            ref var vertex = ref vvs[i];
            vertex = Vector3.Transform(vertex, (Matrix4x4)transform);
            vs[i] = new Vertex(vertex);
        }

        // Make faces with verticies
        var faces = new Face[]
        {
            // Back
            new(vs[0], vs[1], vs[2]),
            new(vs[4], vs[1], vs[2]),
            
            // Right
            new(vs[0], vs[1], vs[3]),
            new(vs[6], vs[1], vs[3]),
            
            // Bottom
            new(vs[0], vs[2], vs[5]),
            new(vs[0], vs[3], vs[5]),
            
            // Left
            new(vs[2], vs[4], vs[7]),
            new(vs[2], vs[5], vs[7]),
            
            // Top
            new(vs[4], vs[1], vs[6]),
            new(vs[4], vs[7], vs[6]),
            
            // Front
            new(vs[3], vs[5], vs[6]),
            new(vs[7], vs[5], vs[6]),
        };

        return new Mesh()
        {
            Vertices = vs.ToList(),
            Faces = faces.ToList(),
        };
    }

    public Vector3 Center { get; set; }

    public Vector3 Scale { get; set; }
}
