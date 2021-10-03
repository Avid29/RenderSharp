using ComputeSharp;
using RenderSharp.RayTracing.HLSL.Components;

namespace RenderSharp.RayTracing.HLSL
{
    [AutoConstructor]
    public readonly partial struct RayTraceShader : IPixelShader<Float4>
    {
        private readonly float time;

        public Float4 Execute()
        {
            Int2 size = DispatchSize.XY;
            Int2 xy = ThreadIds.XY;
            Float2 uv = (Float2)xy / size;

            return new Float4(uv.X, uv.Y, .25f, 1);
        }
    }
}
