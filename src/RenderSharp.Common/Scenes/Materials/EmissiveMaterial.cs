using ComputeSharp;

namespace RenderSharp.Common.Scenes.Materials
{
    public class EmissiveMaterial : IMaterial
    {
        public EmissiveMaterial(Float4 emission, float strength = 1)
        {
            Emission = emission;
            Strength = strength;
        }

        public Float4 Emission { get; }

        public float Strength { get; }
    }
}
