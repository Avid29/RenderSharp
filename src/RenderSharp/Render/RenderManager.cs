using ComputeSharp;
using RenderSharp.Common.Render;
using RenderSharp.Common.Render.Tiles;
using RenderSharp.Common.Scenes;
using RenderSharp.Render.Analyzer;
using RenderSharp.Render.Tiles;
using RenderSharp.WinUI.Renderer;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RenderSharp.Render
{
    public class RenderManager<TRenderer>
        where TRenderer : IRenderer
    {
        private TileManager _tileManager;
        private RenderAnalyzer _analyzer;
        private ReadWriteTexture2D<Float4> _output;

        /// <remarks>
        /// Use property, field may be inaccurate.
        /// </remarks>
        private RenderState _state;

        public RenderManager(TRenderer renderer)
        {
            _state = RenderState.NotReady;
            Renderer = renderer;
            _analyzer = new RenderAnalyzer();
        }

        public bool IsReady => State == RenderState.Ready;

        public bool IsRunning => _state == RenderState.Running || _state == RenderState.Starting;

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

        public async void Render(Scene scene, int width, int height)
        {
            if (!IsReady)
            {
                _state = RenderState.Error;
                return; // TODO: Throw
            }


            _output = Gpu.Default.AllocateReadWriteTexture2D<Float4>( width, height);

            _state = RenderState.Starting;
            _analyzer.Begin();

            // Fire and forget
            await Task.Run(() =>
            {
                // TODO: Take the config as input
                TileConfig defaultConfig = new TileConfig(96*2, 96, TileOrder.TopBottom);
                _tileManager = new TileManager(_output.Width, _output.Height, defaultConfig);
                Renderer.Setup(scene, _output.Width, _output.Height);

                _state = RenderState.Running;
                StartRenderLoops();
            });
        }

        private void StartRenderLoops()
        {
            Thread[] threads = null;
            if (Renderer.IsCPU)
            {
                // Boot other threads
                threads = new Thread[Environment.ProcessorCount - 1];
                for (int i = 0; i < threads.Length; i++)
                {
                    threads[i] = new Thread(() => RenderLoop());
                    threads[i].Start();
                }
            }

            // The main thread is also rendering until it finishes
            RenderLoop();

            // Join the other threads
            if (Renderer.IsCPU)
            {
                foreach (Thread thread in threads)
                {
                    thread.Join();
                }
            }

            _analyzer.End();
        }

        private void RenderLoop()
        {
            while (!_tileManager.Finished)
            {
                Tile tile;
                lock (_tileManager) tile = _tileManager.GetNextTile();

                Renderer.RenderTile(tile);

                // Update
                Renderer.Buffer.CopyToGPU(_output);
            }
        }

        private bool CheckReady()
        {
            bool isReady = true;
            //isReady = isReady && _output != null;
            return isReady;
        }

        public void WriteProgress(IReadWriteTexture2D<Float4> image)
        {
            if (_output == null)
                return;

            Gpu.Default.ForEach(image, new OverlayShaderI(new Int2(0, 0), _output, image));
        }
    }
}
