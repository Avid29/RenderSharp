// Adam Dernis 2023

using ComputeSharp;
using Microsoft.UI.Xaml;
using RenderSharp.Renderers.Debug;
using RenderSharp.Rendering;
using RenderSharp.UI.Shared.Rendering;

namespace RenderSharp.Samples.WinUI;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();

        RenderViewer = new RenderViewer();
    }

    public RenderViewer RenderViewer { get; }

    private void AnimatedComputeShaderPanel_Loaded(object sender, RoutedEventArgs e)
    {
        var renderer = new SimpleRenderer(GraphicsDevice.GetDefault());
        RenderViewer.Setup<RealtimeRenderManager>(renderer);
    }

    private void AnimatedComputeShaderPanel_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        // Skip refresh if initial size change
        if (e.PreviousSize.Width == 0)
            return;

        RenderViewer.Refresh();
    }
}
