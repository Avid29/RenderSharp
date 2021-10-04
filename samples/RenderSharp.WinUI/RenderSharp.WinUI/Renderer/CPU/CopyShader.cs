using ComputeSharp;

namespace RenderSharp.WinUI.Renderer
{
    [AutoConstructor]
    public partial struct CopyShader : IPixelShader<Float4>
    {
        ReadOnlyTexture2D<Float4> texture;

        public Float4 Execute()
        {
            return texture[ThreadIds.XY];
        }
    }
}
