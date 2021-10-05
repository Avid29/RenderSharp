using ComputeSharp;
using RenderSharp.Common.Render;
using RenderSharp.Common.Render.Tiles;
using RenderSharp.Render.Tiles;
using System.Threading.Tasks;

namespace RenderSharp.Render
{
    public class RenderManager<TRenderer>
        where TRenderer : IRenderer
    {
        private TileManager _tileManager;

        /// <remarks>
        /// Use property, field may be inaccurate.
        /// </remarks>
        private RenderState _state;
        private IReadWriteTexture2D<Float4> _output;

        public RenderManager(TRenderer renderer)
        {
            _state = RenderState.NotReady;
            Renderer = renderer;
        }

        public IReadWriteTexture2D<Float4> Output
        {
            get => _output;
            set
            {
                if (IsRunning)
                    return; // TODO: Throw

                _output = value;
            }
        }

        public bool IsReady => State == RenderState.Ready;

        public bool IsRunning => _state == RenderState.Running;

        public bool IsDone => _state == RenderState.Done;

        public RenderState State
        {
            get
            {
                // If the state is NotReady, check and make sure.
                if (_state == RenderState.NotReady && CheckReady()) _state = RenderState.Ready;
                return _state;
            }
        }

        public TRenderer Renderer { get; }

        public async void Begin()
        {
            _state = RenderState.Starting;
            if (!IsReady)
            {
                _state = RenderState.Error;
                return; // TODO: Throw
            }

            _state = RenderState.Running;

            await Task.Run(() => Render());
        }

        private Task Render()
        {
            // TODO: Take the config as input
            TileConfig defaultConfig = new TileConfig(24, 24, TileOrder.TopBottom);

            _tileManager = new TileManager(Output.Width, Output.Height, defaultConfig);

            while (!_tileManager.Finished)
            {
                Tile tile = _tileManager.GetNextTile();
                Renderer.RenderTile(tile);
            }

            return Task.CompletedTask;
        }

        private bool CheckReady()
        {
            bool isReady = true;
            isReady = isReady && Output != null;
            return isReady;
        }
    }
}
