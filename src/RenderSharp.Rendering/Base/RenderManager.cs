// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using ComputeSharp;
using RenderSharp.Rendering.Enums;
using RenderSharp.Rendering.Interfaces;
using RenderSharp.Scenes;
using System.Threading;
using System.Threading.Tasks;

namespace RenderSharp.Rendering.Base;

public class RenderManager
{
    private readonly CancellationTokenSource _cancelTokenSource;
    private IRenderer? _renderer;

    /// <summary>
    /// Initializes a new instance of the <see cref="RenderManager{TRenderer}"/> class.
    /// </summary>
    public RenderManager()
    {
        State = RenderState.NotReady;
        _cancelTokenSource = new CancellationTokenSource();
        RenderAnalyzer = new RenderAnalyzer();
    }

    /// <summary>
    /// Gets the current rendering state.
    /// </summary>
    public RenderState State { get; protected set; }

    protected RenderAnalyzer RenderAnalyzer { get; }

    /// <summary>
    /// Gets the output buffer being rendered to.
    /// </summary>
    public ReadWriteTexture2D<Rgba32, float4>? OutputBuffer { get; protected set; }

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
    public IRenderer? Renderer
    {
        get => _renderer;
        set
        {
            Guard.IsTrue(State is RenderState.NotReady);

            _renderer = value;
            if (_renderer is not null)
            {
                _renderer.RenderAnalyzer = RenderAnalyzer;
            }
        }
    }

    /// <summary>
    /// Loads the 3D scene to the renderer.
    /// </summary>
    public void LoadScene(Scene scene)
    {
        Guard.IsNotNull(Renderer);

        RenderAnalyzer.Begin();
        State = RenderState.Preparing;

        // TODO: Asynchronous
        Renderer.SetupScene(scene);

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
    /// Renders or copies a frame to <paramref name="buffer"/> for display.
    /// </summary>
    /// <param name="buffer">The buffer to display.</param>
    /// <returns>False if the frame should be skipped.</returns>
    public virtual bool FrameUpdate(IReadWriteNormalizedTexture2D<float4> buffer)
    {
        Guard.IsNotNull(OutputBuffer);
        if (OutputBuffer.Width != buffer.Width ||
            OutputBuffer.Height != buffer.Height)
            return false;

        buffer.CopyFrom(OutputBuffer);
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

        OutputBuffer = Device.AllocateReadWriteTexture2D<Rgba32, float4>(width, height);
        Renderer.RenderBuffer = OutputBuffer;
    }

    /// <summary>
    /// Renders an image.
    /// </summary>
    /// <param name="token">A cancellation token to cancel the rendering.</param>
    protected virtual void Render(CancellationToken token)
    {
        Guard.IsNotNull(OutputBuffer);
        Guard.IsNotNull(Renderer);

        Renderer.Render();

        if (token.IsCancellationRequested)
        {
            State = RenderState.Cancelled;
            return;
        }

        RenderAnalyzer.Finish();
        State = RenderState.Done;
    }
}
