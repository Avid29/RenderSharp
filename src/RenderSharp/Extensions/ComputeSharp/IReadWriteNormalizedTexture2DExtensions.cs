// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using RenderSharp.Extensions.ComputeSharp.Shaders;

namespace ComputeSharp;

public static class IReadWriteNormalizedTexture2DExtensions
{
    

    public static void CopyTo(
        this IReadWriteNormalizedTexture2D<float4> source,
        IReadWriteNormalizedTexture2D<float4> destination)
    {
        int2 size = GetMinimumSize(source, destination);
        CopyTo(source, destination, int2.Zero, int2.Zero, size);
    }

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

    public static void CopyFrom(
        this IReadWriteNormalizedTexture2D<float4> destination,
        IReadWriteNormalizedTexture2D<float4> source)
    {
        int2 size = GetMinimumSize(source, destination);
        CopyFrom(destination, source, int2.Zero, int2.Zero, size);
    }

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
