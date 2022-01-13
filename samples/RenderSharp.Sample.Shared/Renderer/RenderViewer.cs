#if NET6_0
using ComputeSharp.WinUI;
#elif WINDOWS_UWP
using ComputeSharp.Uwp;
#endif

using ComputeSharp;
using System;
using RenderSharp.Render;

#nullable enable

namespace RenderSharp.Sample.Shared.Renderer
{
    public class RenderViewer<TRenderer> : IShaderRunner
        where TRenderer : IRenderer
    {
        private TRenderer _renderer;

        public RenderViewer(TRenderer renderer)
        {
            _renderer = renderer;
        }

        public bool TryExecute(IReadWriteTexture2D<Float4> texture, TimeSpan timespan, object? parameter)
        {
            _renderer.Render(texture);
            return true;
        }
    }
}
