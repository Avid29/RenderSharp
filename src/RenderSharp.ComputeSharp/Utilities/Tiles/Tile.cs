// Adam Dernis 2023

namespace RenderSharp.Utilities.Tiles;

/// <summary>
/// A struct for a tile to render.
/// </summary>
public struct Tile
{
    /// <summary>
    /// The tile's offset position.
    /// </summary>
    /// <remarks>
    /// Top left pixel coordinate.
    /// </remarks>
    public int2 offset;

    /// <summary>
    /// The tile's size.
    /// </summary>
    public int2 size;

    /// <summary>
    /// Initializes a new instance if the <see cref="Tile"/> struct.
    /// </summary>
    /// <param name="offset">The tile's offset position.</param>
    /// <param name="size"></param>
    public Tile(int2 offset, int2 size)
    {
        this.offset = offset;
        this.size = size;
    }

    /// <summary>
    /// Gets the X component of the offset.
    /// </summary>
    public int OffsetX => offset.X;
    
    /// <summary>
    /// Gets the Y component of the offset.
    /// </summary>
    public int OffsetY => offset.Y;
    
    /// <summary>
    /// Gets the tile width.
    /// </summary>
    public int Width => size.X;
    
    /// <summary>
    /// Gets the tile height.
    /// </summary>
    public int Height => size.Y;
    
    /// <summary>
    /// Gets the tile offset.
    /// </summary>
    public int2 Offset => offset;
    
    /// <summary>
    /// Gets the tile size.
    /// </summary>
    public int2 Size => size;
    
    /// <summary>
    /// Gets the tile top left pixel coordinate.
    /// </summary>
    public int2 TopLeft => Offset;

    /// <summary>
    /// Gets the tile bottom right pixel coordinate.
    /// </summary>
    public int2 BottomRight => Offset + Size;
}
