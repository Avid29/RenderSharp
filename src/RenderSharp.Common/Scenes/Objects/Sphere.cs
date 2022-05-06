using RenderSharp.Scenes.Materials;
using System.Numerics;

namespace RenderSharp.Scenes.Objects
{
    public class Sphere : IObject
    {
        public Sphere(Vector3 center, float radius, MaterialBase material)
        {
            Center = center;
            Radius = radius;
            Material = material;
        }

        public Vector3 Center { get; }

        public float Radius { get; }

        public MaterialBase Material { get; set; }
    }
}
