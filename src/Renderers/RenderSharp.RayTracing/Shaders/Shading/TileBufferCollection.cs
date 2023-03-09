// Adam Dernis 2023

using ComputeSharp;
using RenderSharp.RayTracing.Models;
using RenderSharp.RayTracing.Models.Geometry;
using RenderSharp.RayTracing.Models.Lighting;
using RenderSharp.RayTracing.Models.Rays;
using RenderSharp.Utilities.Tiles;

namespace RenderSharp.RayTracing.Shaders.Shading;

public struct TileBufferCollection
{
    public TileBufferCollection(
        GraphicsDevice device, Tile tile,
        ReadOnlyBuffer<ObjectSpace> objectBuffer,
        ReadOnlyBuffer<Vertex> vertexBuffer,
        ReadOnlyBuffer<Triangle> geometryBuffer,
        ReadOnlyBuffer<Light> lightBuffer)
    {
        Device = device;
        Tile = tile;
        ObjectBuffer = objectBuffer;
        VertexBuffer = vertexBuffer;
        GeometryBuffer = geometryBuffer;
        LightBuffer = lightBuffer;
        
        var tilePixelCount = tile.Width * tile.Height;

        RayBuffer = Device.AllocateReadWriteBuffer<Ray>(tilePixelCount);
        ShadowCastBuffer = Device.AllocateReadWriteBuffer<Ray>(tilePixelCount * LightBuffer.Length);
        RayCastBuffer = Device.AllocateReadWriteBuffer<GeometryCollision>(tilePixelCount);
        AttenuationBuffer = Device.AllocateReadWriteTexture2D<Rgba32, float4>(Tile.Width, Tile.Height);
        ColorBuffer = Device.AllocateReadWriteTexture2D<Rgba32, float4>(Tile.Width, Tile.Height);

        //bvhStack = Device.AllocateReadWriteTexture3D<int>(Tile.Width, Tile.Height, _bvhDepth + 1);

        RandBuffer = Device.AllocateReadWriteBuffer<Rand>(tilePixelCount);
    }

    public GraphicsDevice Device { get; }

    public Tile Tile { get; }
    
    public ReadOnlyBuffer<ObjectSpace> ObjectBuffer { get; }
    
    public ReadOnlyBuffer<Vertex> VertexBuffer { get; }
    
    public ReadOnlyBuffer<Triangle> GeometryBuffer { get; }

    public ReadOnlyBuffer<Light> LightBuffer { get; }

    public ReadWriteBuffer<Ray> RayBuffer { get; }

    public ReadWriteBuffer<Ray> ShadowCastBuffer { get; }

    public ReadWriteBuffer<GeometryCollision> RayCastBuffer { get; }

    public IReadWriteNormalizedTexture2D<float4> AttenuationBuffer { get; }

    public IReadWriteNormalizedTexture2D<float4> ColorBuffer { get; }

    public ReadWriteBuffer<Rand> RandBuffer { get; }
}
