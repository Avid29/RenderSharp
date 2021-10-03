using ComputeSharp.WinUI;
using Microsoft.UI.Xaml;
using RenderSharp.Renderer;

namespace RenderSharp.WinUI
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

        public IShaderRunner Shader = new ShaderRenderer();
    }
}
