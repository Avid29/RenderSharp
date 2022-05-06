﻿using ComputeSharp;
using RenderSharp.Buffer;
using RenderSharp.Render;
using RenderSharp.Render.Tiles;
using RenderSharp.Scenes;

namespace RenderSharp.RayTracing.CPU
{
    public class CPURayTraceRenderer : IRenderer
    {
        private CPUReadWriteImageBuffer _buffer;
        private RayTracer _rayTracer;

        public IReadWriteImageBuffer Buffer => _buffer;

        public bool IsCPU => false;

        public void RenderTile(Tile tile)
        {
            _rayTracer.RenderTile(tile);
        }

        public void Setup(Scene scene, int imageWidth, int imageHeight)
        {
            _buffer = new CPUReadWriteImageBuffer(imageWidth, imageHeight);

            _rayTracer = new RayTracer(scene, new Int2(imageWidth, imageHeight), _buffer);
        }
    }
}
