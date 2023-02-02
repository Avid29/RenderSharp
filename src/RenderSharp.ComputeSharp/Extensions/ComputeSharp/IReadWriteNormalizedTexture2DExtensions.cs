// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using RenderSharp.Extensions.ComputeSharp.Shaders;

namespace ComputeSharp;

/// <summary>
/// A static class containing extension methods for the <see cref="IReadWriteNormalizedTexture2D{TPixel}"/> interface.
/// </summary>
public static class IReadWriteNormalizedTexture2DExtensions
{
    /// <summary>
    /// Copies data from <paramref name="source"/> to <paramref name="destination"/>.
    /// </summary>
    /// <param name="source">The source texture.</param>
    /// <param name="destination">The destination texture.</param>
    public static void CopyTo(
        this IReadWriteNormalizedTexture2D<float4> source,
        IReadWriteNormalizedTexture2D<float4> destination)
    {
        int2 size = GetMinimumSize(source, destination);
        CopyTo(source, destination, int2.Zero, int2.Zero, size);
    }
    
    /// <summary>
    /// Copies a section data from <paramref name="source"/> to <paramref name="destination"/>.
    /// </summary>
    /// <param name="source">The source texture.</param>
    /// <param name="destination">The destination texture.</param>
    /// <param name="sourceOffset">The offset for the source texture.</param>
    /// <param name="destinationOffset">The offset for the destination texture.</param>
    /// <param name="size">The amount of data to copy.</param>
    public static void CopyTo(
        this IReadWriteNormalizedTexture2D<float4> source,
        IReadWriteNormalizedTexture2D<float4> destination,
        int2 sourceOffset,
        int2 destinationOffset,
        int2 size)
    {
        Guard.IsTrue(source.GraphicsDevice == destination.GraphicsDevice);

        source.GraphicsDevice.For(size.X, size.Y, new CopyShader(source, destination, sourceOffset, destinationOffset));
    }
    
    /// <summary>
    /// Copies data to <paramref name="destination"/> from <paramref name="source"/>.
    /// </summary>
    /// <param name="destination">The destination texture.</param>
    /// <param name="source">The source texture.</param>
    public static void CopyFrom(
        this IReadWriteNormalizedTexture2D<float4> destination,
        IReadWriteNormalizedTexture2D<float4> source)
    {
        int2 size = GetMinimumSize(source, destination);
        CopyFrom(destination, source, int2.Zero, int2.Zero, size);
    }
    
    /// <summary>
    /// Copies data to <paramref name="destination"/> from <paramref name="source"/>.
    /// </summary>
    /// <param name="destination">The destination texture.</param>
    /// <param name="source">The source texture.</param>
    /// <param name="destinationOffset">The offset for the destination texture.</param>
    /// <param name="sourceOffset">The offset for the source texture.</param>
    /// <param name="size">The amount of data to copy.</param>
    public static void CopyFrom(
        this IReadWriteNormalizedTexture2D<float4> destination,
        IReadWriteNormalizedTexture2D<float4> source,
        int2 destinationOffset,
        int2 sourceOffset,
        int2 size)
        => CopyTo(source, destination, sourceOffset, destinationOffset, size);


    private static int2 GetMinimumSize(IReadWriteNormalizedTexture2D<float4> texture1, IReadWriteNormalizedTexture2D<float4> texture2)
    {
        int width = int.Min(texture1.Width, texture2.Width);
        int height = int.Min(texture1.Height, texture2.Height);
        return new int2(width, height);
    }
}
