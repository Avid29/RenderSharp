using Windows.UI.Xaml.Controls;
using RenderSharp.RayTracing.GPU;
using RenderSharp.Sample.Shared.Renderer;

namespace RenderSharp.Sample.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        public RenderViewer<HlslRayTraceRenderer> Shader = new RenderViewer<HlslRayTraceRenderer>(new HlslRayTraceRenderer());
    }
}
