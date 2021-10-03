using ComputeSharp;

namespace RenderSharp.Common.Materials
{
    public class DiffuseMaterial : IMaterial
    {
        public DiffuseMaterial(Float4 albedo)
        {
            Albedo = albedo;
        }

        public Float4 Albedo { get; }
    }
}
