// Adam Dernis 2023

namespace RenderSharp.Utilities.Tiles;

/// <summary>
/// A struct containing configuration data for the <see cref="TileManager"/>.
/// </summary>
public struct TileConfig
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TileConfig"/> struct.
    /// </summary>
    /// <param name="tileSize">The maximum size of a tile</param>
    public TileConfig(int2 tileSize)
    {
        TileSize = tileSize;
        Order = TileOrder.LeftToRight;
    }
    
    /// <summary>
    /// Gets the maximum size of a tile.
    /// </summary>
    public int2 TileSize { get; }

    /// <summary>
    /// Gets the order the tiles are queued in.
    /// </summary>
    public TileOrder Order { get; }
}
