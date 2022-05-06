using ComputeSharp;
using Microsoft.Toolkit.HighPerformance;
using RenderSharp.RayTracing.Scenes;
using RenderSharp.RayTracing.Scenes.Cameras;
using RenderSharp.RayTracing.Scenes.Rays;
using RenderSharp.RayTracing.Utils;
using System;

namespace RenderSharp.RayTracing.CPU.MockShaders
{
    [AutoConstructor]
    public ref partial struct CameraCastMockShader
    {
        private readonly Scene _scene;
        private readonly FullCamera _camera;
        private readonly Int2 _offset;
        private readonly Int2 _fullsize;
        private readonly Span<Ray> _rayBuffer;
        private readonly Span2D<uint> _randStates;

        public void Execute(int width, int height)
        {
            for (int col = 0; col < width; col++)
            {
                int x = _offset.X + col;
                for (int row = 0; row < height; row++)
                {
                    int y = _offset.Y + row;

                    ref uint randState = ref _randStates[col, row];
                    float u = (x + RandUtils.RandomFloat(ref randState)) / _fullsize.X;
                    float v = 1 - ((y + RandUtils.RandomFloat(ref randState)) / _fullsize.Y);

                    Ray ray = FullCamera.CreateRay(_camera, u, v, ref randState);
                    _rayBuffer[row * width + col] = ray;
                }
            }
        }
    }
}
