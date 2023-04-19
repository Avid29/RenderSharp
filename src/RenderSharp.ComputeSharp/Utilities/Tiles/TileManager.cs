// Adam Dernis 2023

namespace RenderSharp.Utilities.Tiles;

/// <summary>
/// A class for managing the sequence tiles are rendered.
/// </summary>
public class TileManager
{
    private readonly TileConfig _config;
    private readonly int2 _imageSize;

    /// <summary>
    /// Initializes a new instance of the <see cref="TileManager"/> class.
    /// </summary>
    /// <param name="config">The tile config.</param>
    /// <param name="imageSize">The size of the image to render.</param>
    public TileManager(TileConfig config, int2 imageSize)
    {
        _config = config;
        _imageSize = imageSize;
        TileCount = DetermineTileCount();
    }

    /// <summary>
    /// Gets the index of the last queued tile.
    /// </summary>
    public int PreviousTileIndex { get; private set; }

    /// <summary>
    /// Gets the total number of tiles to render.
    /// </summary>
    public int TileCount { get; }
    
    /// <summary>
    /// Gets a value indicating whether or not the tile queue is finished.
    /// </summary>
    public bool Finished => !(PreviousTileIndex < TileCount);
    
    /// <summary>
    /// Gets next <see cref="Tile"/> to render.
    /// </summary>
    /// <returns>The next <see cref="Tile"/> to render.</returns>
    public Tile GetNextTile()
    {
        if (Finished)
            return new Tile(); // TODO: Throw 

        // TODO: More ordering options
        Tile tile = _config.Order switch
        {
            _ => NextTileFromSide(_config.Order),
        };
        PreviousTileIndex++;
        return tile;
    }

    private int DetermineTileCount()
    {
        return _config.Order switch
        {
            _ => TileCountFromSide(),
        };
    }

    private Tile NextTileFromSide(TileOrder order)
    {
        int offsetX = _config.TileSize.X;
        int offsetY = _config.TileSize.Y;

        // Round up
        int perRow = (_imageSize.X + offsetX - 1) / offsetX;

        int row = PreviousTileIndex / perRow;
        int col = PreviousTileIndex % perRow;

        (row, col) = order switch
        {
            //TileOrder.TopToBottom => (col, row),
            _ or TileOrder.LeftToRight => (row, col),
        };

        int x = col * offsetX;
        int y = row * offsetY;

        int width = offsetX;
        if (x + width > _imageSize.X)
            width = _imageSize.X - x;

        int height = offsetY;
        if (y + height > _imageSize.Y)
            height = _imageSize.Y - y;

        var offset = new int2(x, y);
        var size = new int2(width, height);

        return new Tile(offset, size);
    }

    private int TileCountFromSide()
    {
        // X/Y, Row/Column changed to ambiguous U/V and Span
        // This is because the distinction between axis is not important

        int offsetU = _config.TileSize.X;
        int offsetV = _config.TileSize.Y;

        // Round up
        int perUSpan = (_imageSize.X + offsetU - 1) / offsetU;
        int perVSpan = (_imageSize.Y + offsetV - 1) / offsetV;

        return perUSpan * perVSpan;
    }
}
