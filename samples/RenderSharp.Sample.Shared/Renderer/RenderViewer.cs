#if NET6_0
using ComputeSharp.WinUI;
#elif WINDOWS_UWP
using ComputeSharp.Uwp;
#endif

using ComputeSharp;
using System;
using RenderSharp.Render;
using RenderSharp.Scenes;

#nullable enable

namespace RenderSharp.Sample.Shared.Renderer
{
    public class RenderViewer<TRenderer> : IShaderRunner
        where TRenderer : IRenderer
    {
        private RenderManager<TRenderer> _renderManager;

        public RenderViewer(TRenderer renderer)
        {
            _renderManager = new RenderManager<TRenderer>(renderer);
        }

        public Scene Scene { get; set; }

        public bool TryExecute(IReadWriteTexture2D<Float4> texture, TimeSpan timespan, object? parameter)
        {
            // Begin render if not begun
            if (!_renderManager.IsRunning) _renderManager.Render(Scene, texture.Width, texture.Height);

            _renderManager.WriteProgress(texture);
            return true;
        }
    }
}
