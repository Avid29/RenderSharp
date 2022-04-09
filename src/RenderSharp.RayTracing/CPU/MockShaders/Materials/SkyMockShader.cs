using ComputeSharp;
using Microsoft.Toolkit.HighPerformance;
using RenderSharp.RayTracing.Scenes;
using RenderSharp.RayTracing.Scenes.Rays;
using System;
using System.Diagnostics;
using System.Numerics;

namespace RenderSharp.RayTracing.CPU.MockShaders.Materials
{
    [AutoConstructor]
    public ref partial struct SkyMockShader
    {
        private readonly Scene _scene;
        private readonly int2 _offset;
        private readonly int2 _tileSize;
        private readonly Vector4 _albedo;

        private readonly Span<Ray> _rayBuffer;
        private readonly Span<RayCast> _rayCastBuffer;
        private readonly Span2D<int> _materialBuffer;

        private readonly Span2D<Vector4> _attenuationBuffer;
        private readonly Span2D<Vector4> _colorBuffer;

        public void Execute(int width, int height)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (_materialBuffer[x, y] != -1) continue;

                    int bPos = x * _tileSize.Y + y;

                    Ray ray = _rayBuffer[bPos];
                    RayCast cast = _rayCastBuffer[bPos];

                    Vector3 unitDirection = Vector3.Normalize(ray.direction);
                    float t = 0.5f * (unitDirection.Y + 1);
                    Vector4 rawColor = (1f - t) * Vector4.One + t * _albedo;

                    Vector4 attenuation = _attenuationBuffer[x, y];
                    _colorBuffer[y + _offset.Y, x + _offset.X] = attenuation * rawColor;
                    _materialBuffer[x, y] = -2;
                }
            }
        }
    }
}
