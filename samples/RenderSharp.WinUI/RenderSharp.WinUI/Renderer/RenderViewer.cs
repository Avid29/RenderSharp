using ComputeSharp;
using ComputeSharp.WinUI;
using RenderSharp.Common.Render;
using RenderSharp.Common.Scenes;
using RenderSharp.Render;
using System;

namespace RenderSharp.WinUI.Renderer
{
    public class RenderViewer<TRenderer> : IShaderRunner
        where TRenderer : IRenderer
    {
        private RenderManager<TRenderer> _renderManager;

        public void Setup(TRenderer renderer)
        {
            _renderManager = new RenderManager<TRenderer>(renderer);
        }

        public Scene Scene { get; set; }

        public bool TryExecute(IReadWriteTexture2D<Float4> texture, TimeSpan timespan, object parameter)
        {
            // Begin render if not begun
            if (!_renderManager.IsRunning) _renderManager.Render(Scene, texture.Width, texture.Height);

            _renderManager.WriteProgress(texture);
            return true;
        }
    }
}
