using ComputeSharp;
using Microsoft.Toolkit.HighPerformance;
using RenderSharp.RayTracing.Scenes;
using RenderSharp.RayTracing.Scenes.Materials;
using RenderSharp.RayTracing.Scenes.Rays;
using RenderSharp.RayTracing.Utils;
using System;
using System.Numerics;

namespace RenderSharp.RayTracing.CPU.MockShaders.Materials
{
    [AutoConstructor]
    public ref partial struct DiffuseMockShader
    {
        private readonly int _matId;

        private readonly Scene _scene;
        private readonly int2 _offset;
        private readonly int2 _tileSize;
        private readonly DiffuseMaterial _material;

        private readonly Span<Ray> _rayBuffer;
        private readonly Span<RayCast> _rayCastBuffer;
        private readonly Span2D<int> _materialBuffer;

        private readonly Span2D<float4> _attenuationBuffer;
        private readonly Span2D<float4> _colorBuffer;
        private readonly Span2D<uint> _randStates;

        public void Execute(int width, int height)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (_materialBuffer[x, y] != _matId) return;
                    int bPos = x * _tileSize.X + y;
                    uint randState = _randStates[x, y];

                    RayCast cast = _rayCastBuffer[bPos];

                    Vector3 target = cast.origin + cast.normal;
                    target += _material.roughness * RandUtils.RandomInUnitSphere(ref randState);
                    Ray scatter = Ray.Create(cast.origin, target - cast.origin);

                    _rayBuffer[bPos] = scatter;
                    _attenuationBuffer[x, y] *= _material.albedo;
                    _randStates[x, y] = randState;
                }
            }
        }
    }
}
