﻿// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using ComputeSharp;
using RenderSharp.Rendering.Manager.Base;
using System.Threading;

namespace RenderSharp.Rendering.Manager;

/// <summary>
/// A <see cref="RenderManagerBase"/> for images that render in realtime.
/// </summary>
public class RealtimeRenderManager : RenderManagerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RealtimeRenderManager"/> class.
    /// </summary>
    public RealtimeRenderManager()
    {
    }

    /// <inheritdoc/>
    protected override void AllocateBuffer(int width, int height)
    {
        // No additional buffer needed for realtime rendering.
    }

    /// <inheritdoc/>
    protected override void Render(CancellationToken token)
    {
        // All rendering is done in RenderFrame for realtime rendering
    }

    /// <inheritdoc/>
    public override bool FrameUpdate(IReadWriteNormalizedTexture2D<float4> buffer)
    {
        Guard.IsNotNull(Renderer);

        Renderer.RenderBuffer = buffer;
        Renderer.Render();
        return true;
    }
}