using ComputeSharp;
using Microsoft.Toolkit.HighPerformance;
using RenderSharp.RayTracing.Scenes;
using System.Numerics;

namespace RenderSharp.RayTracing.CPU.MockShaders
{
    [AutoConstructor]
    public ref partial struct InitializeMockShader
    {
        private readonly Scene _scene;
        private readonly int2 _offset;

        private readonly Span2D<Vector4> _attenuationBuffer;
        private readonly Span2D<uint> _randStates;

        public void Execute()
        {
            _attenuationBuffer.Fill(Vector4.One);
            int s = _scene.config.samples;

            for (int row = 0; row < _randStates.Height; row++)
            {
                int x = row + _offset.X;
                for (int col = 0; col < _randStates.Width; col++)
                {
                    int y = col + _offset.Y;
                    _randStates[row, col] = (uint)(x * 1973 + y * 9277 + s * 26699) | 1;
                }
            }
        }
    }
}
