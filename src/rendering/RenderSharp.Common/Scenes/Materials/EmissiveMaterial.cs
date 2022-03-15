using System.Numerics;

namespace RenderSharp.Scenes.Materials
{
    public class EmissiveMaterial : IMaterial
    {
        public EmissiveMaterial(Vector4 emission, float strength = 1)
        {
            Emission = emission;
            Strength = strength;
        }

        public Vector4 Emission { get; }

        public float Strength { get; }
    }
}
