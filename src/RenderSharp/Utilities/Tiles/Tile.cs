// Adam Dernis 2023

namespace RenderSharp.Utilities.Tiles;

public struct Tile
{
    public int2 offset;
    public int2 size;

    public Tile(int2 offset, int2 size)
    {
        this.offset = offset;
        this.size = size;
    }

    public int OffsetX => offset.X;

    public int OffsetY => offset.Y;

    public int Width => size.X;

    public int Height => size.Y;

    public int2 Offset => offset;

    public int2 Size => size;

    public int2 TopLeft => Offset;

    public int2 BottomRight => Offset + Size;
}
