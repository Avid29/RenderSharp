using ComputeSharp;

namespace RenderSharp.Render.Tiles
{
    public struct Tile
    {
        private int _offsetX;
        private int _offsetY;
        private int _width;
        private int _height;

        public Tile(int offsetX, int offsetY, int width, int height)
        {
            _offsetX = offsetX;
            _offsetY = offsetY;
            _width = width;
            _height = height;
        }

        public int OffsetX => _offsetX;

        public int OffsetY => _offsetY;

        public int Width => _width;

        public int Height => _height;

        public Int2 Offset => new Int2(OffsetX, OffsetY);

        public Int2 Size => new Int2(Width, Height);

        public Int2 XY1 => Offset;

        public Int2 XY2 => Offset + Size;
    }
}
