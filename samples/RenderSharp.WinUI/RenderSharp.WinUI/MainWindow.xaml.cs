using Microsoft.UI.Xaml;
using RenderSharp.Common.Components;
using RenderSharp.Renderer;
using RenderSharp.WinUI.Renderer;

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

            Scene scene = Scene.CreateSpheresScene();
            Shader.AllocateResources(scene);
        }

        public ISceneRenderer Shader = new ProgressiveRenderer<ShaderRenderer>(new ShaderRenderer());
    }
}
