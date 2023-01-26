﻿// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using ComputeSharp;
using System.Threading;
using System.Threading.Tasks;

namespace RenderSharp.Rendering;

public class RenderManager
{
    private readonly CancellationTokenSource _cancelTokenSource;
    private ReadWriteTexture2D<Rgba32, float4>? _output;

    /// <summary>
    /// Initializes a new instance of the <see cref="RenderManager{TRenderer}"/> class.
    /// </summary>
    public RenderManager()
    {
        State = RenderState.NotReady;
        _cancelTokenSource = new CancellationTokenSource();
    }

    /// <summary>
    /// Gets the current rendering state.
    /// </summary>
    public RenderState State { get; private set; }

    /// <summary>
    /// Gets a value indicating if the renderer is ready.
    /// </summary>
    public bool IsReady => State is RenderState.Ready;

    /// <summary>
    /// Gets a value indicating if the renderer is running.
    /// </summary>
    public bool IsRunning => State is RenderState.Running;

    /// <summary>
    /// Gets a value indicating if the render has finished.
    /// </summary>
    public bool IsDone => State is RenderState.Done;

    /// <summary>
    /// Gets the <see cref="GraphicsDevice"/> being used by the renderer.
    /// </summary>
    public GraphicsDevice? Device => Renderer?.Device;

    /// <summary>
    /// Gets the underlying renderer.
    /// </summary>
    public IRenderer? Renderer { get; private set; }

    /// <summary>
    /// Loads the 3D scene to the 
    /// </summary>
    public void LoadScene()
    {
        State = RenderState.Preparing;
        // TODO: Load a scene into the renderer.
        State = RenderState.Ready;
    }

    /// <summary>
    /// Begins rendering an image.
    /// </summary>
    /// <param name="width">The width of the image.</param>
    /// <param name="height">The height of the image.</param>
    public virtual void Begin(int width, int height)
    {
        if (!IsReady)
        {
            State = RenderState.Error;
            return;
        }

        AllocateBuffer(width, height);
        State = RenderState.Running;
        _ = Task.Run(() => Render(_cancelTokenSource.Token));
    }

    /// <summary>
    /// Resets the render back to before running <see cref="Begin(int, int)"/>.
    /// </summary>
    public void Reset()
    {
        if (State is not RenderState.NotReady)
            State = RenderState.Ready;
    }

    /// <summary>
    /// Cancels a render.
    /// </summary>
    public void Cancel()
    {
        if (!IsRunning)
            return;

        State = RenderState.Cancelling;
        _cancelTokenSource.Cancel();
    }

    /// <summary>
    /// Sets the render, if rendering is not already setup.
    /// </summary>
    /// <param name="renderer">The new renderer.</param>
    public void SetRenderer(IRenderer renderer)
    {
        if (State is RenderState.NotReady)
        {
            Renderer = renderer;
        }
    }

    /// <summary>
    /// Renders or copies a frame to <paramref name="buffer"/> for display.
    /// </summary>
    /// <param name="buffer">The buffer to display.</param>
    /// <returns>False if the frame should be skipped.</returns>
    public virtual bool RenderFrame(IReadWriteNormalizedTexture2D<float4> buffer)
    {
        Guard.IsNotNull(_output);
        if (_output.Width != buffer.Width ||
            _output.Height != buffer.Height)
            return false;

        buffer.CopyFrom(_output);
        return true;
    }

    /// <summary>
    /// Allocates an additional buffer for non-realtime renders.
    /// </summary>
    /// <param name="width">The width of the buffer.</param>
    /// <param name="height">The height of the buffer.</param>
    protected virtual void AllocateBuffer(int width, int height)
    {
        Guard.IsNotNull(Renderer);
        Guard.IsNotNull(Device);

        _output = Device.AllocateReadWriteTexture2D<Rgba32, float4>(width, height);
        Renderer.RenderBuffer = _output;
    }

    /// <summary>
    /// Renders an image.
    /// </summary>
    /// <param name="token">A cancellation token to cancel the rendering.</param>
    protected virtual void Render(CancellationToken token)
    {
        Guard.IsNotNull(_output);
        Guard.IsNotNull(Renderer);

        Renderer.Render();

        if (token.IsCancellationRequested)
        {
            State = RenderState.Cancelled;
            return;
        }

        State = RenderState.Done;
    }
}