﻿// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using ComputeSharp;
using RenderSharp.RayTracing.Models.Camera;
using RenderSharp.RayTracing.Models.Geometry;
using RenderSharp.RayTracing.Models.Rays;
using RenderSharp.RayTracing.Shaders;
using RenderSharp.RayTracing.Shaders.Debugging;
using RenderSharp.RayTracing.Shaders.Debugging.Enums;
using RenderSharp.Rendering;
using System.Numerics;

namespace RenderSharp.RayTracing;

public class RayTracingRenderer : IRenderer
{
    private ReadOnlyBuffer<Triangle>? _geometryBuffer;

    public RayTracingRenderer(GraphicsDevice device)
    {
        Device = device;
    }

    public GraphicsDevice Device { get; }

    public IReadWriteNormalizedTexture2D<float4>? RenderBuffer { get; set; }

    /// <inheritdoc/>
    public void SetupScene()
    {
        // TODO: Load 3D scene from RenderSharp common scene data

        // Tessellate Cube
        var v0 = Vector3.Zero;
        var v1 = Vector3.UnitX;
        var v2 = Vector3.UnitY;
        var v3 = Vector3.UnitZ;
        var v4 = v1 + v2;
        var v5 = v2 + v3;
        var v6 = v1 + v3;
        var v7 = Vector3.One;

        // TODO: Place proper vertices
        var triangles = new Triangle[]
        {
            new(v1, v2, v3),
        };

        _geometryBuffer = Device.AllocateReadOnlyBuffer(triangles);
    }

    /// <inheritdoc/>
    public void Render()
    {
        Guard.IsNotNull(RenderBuffer);
        Guard.IsNotNull(_geometryBuffer);

        int width = RenderBuffer.Width;
        int height = RenderBuffer.Height;
        float ratio = (float)width / height;
        int2 size = new(width, height);
        int pixelCount = width * height;

        // (Debug) Create camera
        var camera = new Camera(Vector3.UnitZ * 2, Vector3.UnitY * 0.5f, 90f, ratio);

        // Allocate buffers
        ReadWriteBuffer<Ray> rayBuffer = Device.AllocateReadWriteBuffer<Ray>(pixelCount);
        ReadWriteBuffer<RayCast> rayCastBuffer = Device.AllocateReadWriteBuffer<RayCast>(pixelCount);

        // Create the rays from the camera
        Device.For(width, height, new CameraCastShader(size, camera, rayBuffer));

        // Find object collision and cache the resulting ray cast 
        Device.For(width, height, new GeometryCollisionShader(_geometryBuffer, rayBuffer, rayCastBuffer));

        // Dump the ray cast's directions to the render buffer (for debugging)
        Device.For(width, height, new RayCastBufferDumpShader(rayCastBuffer, RenderBuffer, (int)RayCastDumpValueType.Object, _geometryBuffer.Length));
    }

    /// <inheritdoc/>
    public void RenderSegment(int2 offset, int2 size)
    {
        Guard.IsNotNull(RenderBuffer);

        throw new NotImplementedException();
    }
}