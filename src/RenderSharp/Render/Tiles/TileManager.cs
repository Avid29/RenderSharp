using RenderSharp.Common.Render.Tiles;
using RenderSharp.Render.Tiles;

namespace RenderSharp.Render
{
    public class TileManager
    {
        private TileConfig _config;
        private int _imageWidth;
        private int _imageHeight;

        public TileManager(int imageWidth, int imageHeight, TileConfig config)
        {
            _imageWidth = imageWidth;
            _imageHeight = imageHeight;
            _config = config;
            PreviousTileIndex = 0;
            TileCount = DetermineTileCount();
        }

        public int PreviousTileIndex { get; private set; }

        public int TileCount { get; }

        public bool Finished => !(PreviousTileIndex < TileCount);

        public Tile GetNextTile()
        {
            if (PreviousTileIndex > TileCount - 1)
                return new Tile(0, 0, 0, 0); // TODO: Throw 

            // TODO: More ordering options
            Tile tile = NextTileFromSide(_config.Order);
            PreviousTileIndex++;
            return tile;
        }

        private int DetermineTileCount()
        {
            // TODO: More ordering options
            return GetTileCountFromSide();
        }

        private Tile NextTileFromSide(TileOrder order)
        {
            int offsetX = _config.TileWidth;
            int offsetY = _config.TileHeight;

            // Round up
            int perRow = (_imageWidth + offsetX - 1) / offsetX;

            int row = PreviousTileIndex / perRow;
            int col = PreviousTileIndex % perRow;

            if (order == TileOrder.LeftRight)
            {
                int swap = row;
                row = col;
                col = swap;
            }

            int x = col * offsetX;
            int y = row * offsetY;

            int width = offsetX;
            if (x + width > _imageWidth) width = _imageWidth - x;

            int height = offsetY;
            if (y + height > _imageHeight) height = _imageHeight - y;

            return new Tile(x, y, width, height);
        }

        private int GetTileCountFromSide()
        {
            // X/Y, Row/Column changed to ambiguous U/V and Span
            // This is because the distinction is not important

            int offsetU = _config.TileWidth;
            int offsetV = _config.TileHeight;

            // Round up
            int perUSpan = (_imageWidth + offsetU - 1) / offsetU;
            int perVSpan = (_imageHeight + offsetV - 1) / offsetV;

            return perUSpan * perVSpan;
        }
    }
}
