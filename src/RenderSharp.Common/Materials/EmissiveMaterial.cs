using ComputeSharp;

namespace RenderSharp.Common.Materials
{
    public class EmissiveMaterial : IMaterial
    {
        public EmissiveMaterial(Float4 emission)
        {
            Emission = emission;
        }

        public Float4 Emission { get; }
    }
}
