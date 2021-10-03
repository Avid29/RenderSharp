using ComputeSharp;
using System.Threading.Tasks;

namespace RenderSharp.RayTracing.CPU
{
    public class RayTracer
    {
        Int2 _size;

        public Float4 Execute(Int2 pos)
        {
            float u = (float)pos.X / _size.X;
            float v = (float)pos.Y / _size.Y;

            return new Float4(u, v, .25f, 1);
        }

        public Float4[,] Render(Int2 size)
        {
            _size = size;
            Float4[,] frame = new Float4[size.Y, size.X];
            Parallel.For(0, size.Y, y =>
            {
                for (int x = 0; x < size.X; x++)
                {
                    frame[y,x] = Execute(new Int2(x, y));
                }
            });

            return frame;
        }
    }
}
