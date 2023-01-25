// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using ComputeSharp;
using System.Threading;
using System.Threading.Tasks;

namespace RenderSharp.Rendering;

public class RenderManager<TRenderer>
    where TRenderer : IRenderer
{
    private readonly IRenderer _renderer;
    private readonly CancellationTokenSource _cancelTokenSource;
    private ReadWriteTexture2D<Rgba32, float4>? _output;

    /// <summary>
    /// Initializes a new instance of the <see cref="RenderManager{TRenderer}"/> class.
    /// </summary>
    public RenderManager(TRenderer renderer)
    {
        State = RenderState.NotReady;
        _renderer = renderer;
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
    public GraphicsDevice Device => _renderer.Device;

    public void LoadScene()
    {
        State = RenderState.Preparing;
        // TODO: Load a scene into the renderer.
        State = RenderState.Ready;
    }

    public void Begin(int width, int height)
    {
        if (!IsReady)
        {
            State = RenderState.Error;
            return;
        }

        _output = Device.AllocateReadWriteTexture2D<Rgba32, float4>(width, height);
        _renderer.Setup(_output);
        State = RenderState.Running;
        _ = Task.Run(() => Render(width, height, _cancelTokenSource.Token));
    }

    public void Reset()
    {
        if (State is not RenderState.NotReady)
            State = RenderState.Ready;
    }

    public void Cancel()
    {
        if (!IsRunning)
            return;

        State = RenderState.Cancelling;
        _cancelTokenSource.Cancel();
    }

    protected virtual void Render(int width, int height, CancellationToken token)
    {
        Guard.IsNotNull(_output);
        _renderer.Render();

        if (token.IsCancellationRequested)
        {
            State = RenderState.Cancelled;
            return;
        }

        State = RenderState.Done;
    }

    public void RenderFrame(IReadWriteNormalizedTexture2D<float4> buffer)
    {
        // TODO: Handle realtime rendering

        Guard.IsNotNull(_output);
        buffer.CopyFrom(_output);
    }
}
