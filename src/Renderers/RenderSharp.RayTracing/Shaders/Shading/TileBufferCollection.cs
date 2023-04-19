// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Models;
using RenderSharp.RayTracing.Models.BVH;
using RenderSharp.RayTracing.Models.Geometry;
using RenderSharp.RayTracing.Models.Lighting;
using RenderSharp.RayTracing.RayCasts;
using RenderSharp.Utilities.Tiles;

namespace RenderSharp.RayTracing.Shaders.Shading;

/// <summary>
/// A struct containing the buffers required to render a tile.
/// </summary>
public struct TileBufferCollection
{
    /// <inheritdoc cref="TileBufferCollection.TileBufferCollection(GraphicsDevice, Tile, ReadOnlyBuffer{ObjectSpace}, ReadOnlyBuffer{Vertex}, ReadOnlyBuffer{Triangle}, ReadOnlyBuffer{BVHNode}, ReadOnlyBuffer{Light}, int)"/>
    public TileBufferCollection(
        GraphicsDevice device, Tile tile,
        ReadOnlyBuffer<ObjectSpace> objectBuffer,
        ReadOnlyBuffer<Vertex> vertexBuffer,
        ReadOnlyBuffer<Triangle> geometryBuffer,
        ReadOnlyBuffer<Light> lightBuffer) :
        this(device, tile, objectBuffer, vertexBuffer, geometryBuffer, null!, lightBuffer, 0)
    {
        // Suppressed nullability convention. Behavior handled below.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TileBufferCollection"/> struct.
    /// </summary>
    /// <param name="device">The <see cref="GraphicsDevice"/> to allocate the buffer to.</param>
    /// <param name="tile">The tile being rendered.</param>
    /// <param name="objectBuffer">The scene object buffer.</param>
    /// <param name="vertexBuffer">The scene vertex buffer.</param>
    /// <param name="geometryBuffer">The scene geometry buffer.</param>
    /// <param name="bvhTreeBuffer">The scene bvh tree buffer.</param>
    /// <param name="lightBuffer">The scene light buffer.</param>
    /// <param name="bvhDepth">The max depth of the BVH Tree.</param>
    public TileBufferCollection(
        GraphicsDevice device, Tile tile,
        ReadOnlyBuffer<ObjectSpace> objectBuffer,
        ReadOnlyBuffer<Vertex> vertexBuffer,
        ReadOnlyBuffer<Triangle> geometryBuffer,
        ReadOnlyBuffer<BVHNode> bvhTreeBuffer,
        ReadOnlyBuffer<Light> lightBuffer,
        int bvhDepth)
    {
        Device = device;
        Tile = tile;
        ObjectBuffer = objectBuffer;
        VertexBuffer = vertexBuffer;
        GeometryBuffer = geometryBuffer;
        LightBuffer = lightBuffer;
        
        var tilePixelCount = tile.Width * tile.Height;

        PathRayBuffer = Device.AllocateReadWriteBuffer<Ray>(tilePixelCount);
        PathCastBuffer = Device.AllocateReadWriteBuffer<GeometryCollision>(tilePixelCount);
        ShadowRayBuffer = Device.AllocateReadWriteBuffer<Ray>(tilePixelCount * LightBuffer.Length);
        ShadowCastBuffer = Device.AllocateReadWriteBuffer<GeometryCollision>(tilePixelCount * LightBuffer.Length);
        AttenuationBuffer = Device.AllocateReadWriteTexture2D<Rgba32, float4>(Tile.Width, Tile.Height);
        LuminanceBuffer = Device.AllocateReadWriteTexture2D<Rgba32, float4>(Tile.Width, Tile.Height);
        RandStateBuffer = Device.AllocateReadWriteBuffer<Rand>(tilePixelCount);

        // BVHStack 
        BVHTreeBuffer = null;
        BVHStackBuffer = null;

        // Handle nullability behavior because internal call knowingly breaks convention
        if (bvhDepth != 0 && bvhTreeBuffer is not null)
        {
            BVHTreeBuffer = bvhTreeBuffer;
            BVHStackBuffer = Device.AllocateReadWriteBuffer<int>(tilePixelCount * LightBuffer.Length * (bvhDepth + 1));
        }
    }

    /// <summary>
    /// Gets the <see cref="GraphicsDevice"/> the buffers are allocated to.
    /// </summary>
    public GraphicsDevice Device { get; }

    /// <summary>
    /// Gets the tile being rendered.
    /// </summary>
    public Tile Tile { get; }

    /// <summary>
    /// Gets a value indicating whether or not the BVH buffers are allocated.
    /// </summary>
    public bool AreBVHBuffersAllocated => BVHTreeBuffer is not null && BVHStackBuffer is not null;

    /// <summary>
    /// Gets the scene object buffer.
    /// </summary>
    public ReadOnlyBuffer<ObjectSpace> ObjectBuffer { get; }
    
    /// <summary>
    /// Gets the scene vertex buffer.
    /// </summary>
    public ReadOnlyBuffer<Vertex> VertexBuffer { get; }
    
    /// <summary>
    /// Gets the scene geometry buffer.
    /// </summary>
    public ReadOnlyBuffer<Triangle> GeometryBuffer { get; }
    
    /// <summary>
    /// Gets the scene BVH tree buffer.
    /// </summary>
    public ReadOnlyBuffer<BVHNode>? BVHTreeBuffer { get; }

    /// <summary>
    /// Gets the scene light buffer.
    /// </summary>
    public ReadOnlyBuffer<Light> LightBuffer { get; }

    /// <summary>
    /// Gets the tile path ray buffer.
    /// </summary>
    public ReadWriteBuffer<Ray> PathRayBuffer { get; }

    /// <summary>
    /// Gets the tile path cast buffer.
    /// </summary>
    public ReadWriteBuffer<GeometryCollision> PathCastBuffer { get; }

    /// <summary>
    /// Gets the tile shadow ray buffer.
    /// </summary>
    public ReadWriteBuffer<Ray> ShadowRayBuffer { get; }

    /// <summary>
    /// Gets the tile shadow cast buffer.
    /// </summary>
    public ReadWriteBuffer<GeometryCollision> ShadowCastBuffer { get; }

    /// <summary>
    /// Gets the tile attenuation buffer.
    /// </summary>
    public IReadWriteNormalizedTexture2D<float4> AttenuationBuffer { get; }

    /// <summary>
    /// Gets the tile luminance buffer.
    /// </summary>
    public IReadWriteNormalizedTexture2D<float4> LuminanceBuffer { get; }

    /// <summary>
    /// Gets the tile bvh stack buffer.
    /// </summary>
    public ReadWriteBuffer<int>? BVHStackBuffer { get; }

    /// <summary>
    /// Gets the tile rand state buffer.
    /// </summary>
    public ReadWriteBuffer<Rand> RandStateBuffer { get; }
}
