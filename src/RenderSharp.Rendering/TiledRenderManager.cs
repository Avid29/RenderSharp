// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using RenderSharp.Rendering.Base;
using RenderSharp.Rendering.Enums;
using RenderSharp.Rendering.Interfaces;
using RenderSharp.Utilities.Tiles;
using System.Threading;

namespace RenderSharp.Rendering;

public class TiledRenderManager : RenderManager
{
    private TileManager? _tileManager;

    public TiledRenderManager()
    {
    }

    protected override void AllocateBuffer(int width, int height)
    {
        base.AllocateBuffer(width, height);

        var config = new TileConfig(new int2(64, 64));
        _tileManager = new TileManager(config, new int2(width, height));
    }

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

        RenderAnalyzer.Finish();
        State = RenderState.Done;
    }

    private void RenderTile(Tile tile)
    {
        Guard.IsNotNull(Renderer);

        Renderer.RenderSegment(tile);
    }
}
