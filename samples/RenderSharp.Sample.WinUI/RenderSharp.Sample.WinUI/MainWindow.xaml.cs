using Microsoft.UI.Xaml;
using RenderSharp.RayTracing.GPU;
using RenderSharp.Sample.Shared.Renderer;

namespace RenderSharp.Sample.WinUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        public RenderViewer<HlslRayTraceRenderer> Shader = new RenderViewer<HlslRayTraceRenderer>(new HlslRayTraceRenderer());
    }
}
