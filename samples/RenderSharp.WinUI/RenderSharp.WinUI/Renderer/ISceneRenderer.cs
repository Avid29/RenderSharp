using ComputeSharp;
using ComputeSharp.WinUI;
using RenderSharp.Common.Components;

namespace RenderSharp.Renderer
{
    public interface ISceneRenderer : IShaderRunner
    {
        public void AllocateResources(Scene scene);
    }
}
