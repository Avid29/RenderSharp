using ComputeSharp;
using Microsoft.UI.Xaml;
using RenderSharp.Renderers.Debug;
using RenderSharp.UI.Shared.Rendering;
using System.Timers;

namespace RenderSharp.Samples.WinUI;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();

        var renderer = new SimpleRenderer(GraphicsDevice.GetDefault());
        RenderViewer = new RenderViewer<SimpleRenderer>(renderer);
    }

    public RenderViewer<SimpleRenderer> RenderViewer { get; }

    private void AnimatedComputeShaderPanel_Loaded(object sender, RoutedEventArgs e)
    {
        RenderViewer.Setup();
    }

    private void AnimatedComputeShaderPanel_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.PreviousSize.Width == 0)
            return;

        RenderViewer.Refresh();
        RenderViewer.Setup();
    }
}
