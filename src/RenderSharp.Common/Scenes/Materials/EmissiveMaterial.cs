using System.Numerics;

namespace RenderSharp.Scenes.Materials
{
    public class EmissiveMaterial : MaterialBase
    {
        public EmissiveMaterial(Vector4 emission, float strength = 1) :
            this(string.Empty, emission, strength)
        {
            Emission = emission;
            Strength = strength;
        }

        public EmissiveMaterial(string name, Vector4 emission, float strength = 1) :
            base(name)
        {
            Emission = emission;
            Strength = strength;
        }

        public Vector4 Emission { get; }

        public float Strength { get; }
    }
}
