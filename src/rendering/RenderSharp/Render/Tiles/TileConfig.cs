namespace RenderSharp.Render.Tiles
{
    public struct TileConfig
    {
        public TileConfig(int tileWidth, int tileHeight, TileOrder order)
        {
            TileWidth = tileWidth;
            TileHeight = tileHeight;
            Order = order;
        }

        public int TileWidth { get; }

        public int TileHeight { get; }

        public TileOrder Order { get; }
    }
}
