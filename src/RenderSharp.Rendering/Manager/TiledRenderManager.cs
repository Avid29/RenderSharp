// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using RenderSharp.Rendering.Manager.Base;
using RenderSharp.Rendering.Manager.Enums;
using RenderSharp.Utilities.Tiles;
using System.Threading;

namespace RenderSharp.Rendering.Manager;

/// <summary>
/// A <see cref="RenderManagerBase"/> for rendering an image in tiles.
/// </summary>
public class TiledRenderManager : RenderManagerBase
{
    private TileManager? _tileManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="TiledRenderManager"/> class.
    /// </summary>
    public TiledRenderManager()
    {
        TileConfig = new TileConfig(new int2(64, 64));
    }

    /// <summary>
    /// Gets or sets the tile configurations.
    /// </summary>
    public TileConfig TileConfig { get; set; }

    /// <inheritdoc/>
    protected override void AllocateBuffer(int width, int height)
    {
        base.AllocateBuffer(width, height);

        _tileManager = new TileManager(TileConfig, new int2(width, height));
    }

    /// <inheritdoc/>
    protected override void Render(CancellationToken token)
    {
        Guard.IsNotNull(OutputBuffer);
        Guard.IsNotNull(Renderer);
        Guard.IsNotNull(_tileManager);

        while (!_tileManager.Finished)
        {
            if (token.IsCancellationRequested)
            {
                State = RenderState.Cancelled;
                return;
            }

            var tile = _tileManager.GetNextTile();
            RenderTile(tile);
        }

        PostProcessor?.Process(OutputBuffer);

        RenderAnalyzer.Finish();
        State = RenderState.Done;
    }

    private void RenderTile(Tile tile)
    {
        Guard.IsNotNull(Renderer);
        Renderer.RenderSegment(tile);
    }
}
