// Adam Dernis 2023

namespace RenderSharp.Utilities.Tiles;

public struct TileConfig
{
    public TileConfig(int2 tileSize)
    {
        TileSize = tileSize;
        Order = TileOrder.LeftToRight;
    }

    public int2 TileSize { get; }

    public TileOrder Order { get; }
}
