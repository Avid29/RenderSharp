// Adam Dernis 2023

using ComputeSharp;
using Microsoft.UI.Xaml;
using RenderSharp.RayTracing;
using RenderSharp.Rendering;
using RenderSharp.ToneReproduction;
using RenderSharp.UI.Shared.Rendering;
using TerraFX.Interop.Windows;

namespace RenderSharp.Samples.WinUI;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        this.InitializeComponent();

        RenderViewer = new RenderViewer();
    }

    /// <summary>
    /// Gets the renderer viewer for the window.
    /// </summary>
    public RenderViewer RenderViewer { get; }

    private void AnimatedComputeShaderPanel_Loaded(object sender, RoutedEventArgs e)
    {
        var renderer = new RayTracingRenderer();
        var postProcessor = new ToneReproducer();
        RenderViewer.Setup<TiledRenderManager>(renderer, postProcessor);
    }

    private void AnimatedComputeShaderPanel_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        // Skip refresh if initial size change
        if (e.PreviousSize.Width == 0)
            return;

        RenderViewer.Refresh();
    }
}
