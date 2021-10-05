using ComputeSharp;
using ComputeSharp.WinUI;
using RenderSharp.Common.Scenes;

namespace RenderSharp.Renderer
{
    public interface ISceneRenderer : IShaderRunner
    {
        public void AllocateResources(Scene scene);
    }
}
