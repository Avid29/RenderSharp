﻿using ComputeSharp;
using RenderSharp.Renderer;
using System;
using CommonScene = RenderSharp.Common.Scenes.Scene;

namespace RenderSharp.WinUI.Renderer
{
    public class ProgressiveRenderer<T> : ISceneRenderer
        where T : ITileRenderer
    {
        private readonly GraphicsDevice _gpu;
        private T _tileRenderer;

        private int currentTile = 0;
        private int tileCount = -1;
        private bool done = false;
        private int tileHeight = 25;

        public ProgressiveRenderer(T tileRenderer) : this(Gpu.Default, tileRenderer)
        { }

        public ProgressiveRenderer(GraphicsDevice gpu, T tileRenderer)
        {
            _gpu = gpu;
            _tileRenderer = tileRenderer;
        }

        public void AllocateResources(CommonScene scene)
        {
            _tileRenderer.AllocateResources(scene);
        }

        public void Execute(IReadWriteTexture2D<Float4> texture, TimeSpan timespan)
        {
            if (tileCount != MathF.Ceiling((float)texture.Height / tileHeight))
            {
                currentTile = 0;
                tileCount = (int)MathF.Ceiling((float)texture.Height / tileHeight);
                done = false;
            }

            if (currentTile == tileCount) done = true;

            if (done) return;

            Int2 offset = new Int2(0, currentTile * tileHeight);
            Int2 tileSize = new Int2(texture.Width, tileHeight);

            if (offset.Y + tileSize.Y > texture.Height)
            {
                tileSize.Y = texture.Height - offset.Y;
            }

            _tileRenderer.Render(texture, tileSize, offset);

            currentTile++;
        }
    }
}
