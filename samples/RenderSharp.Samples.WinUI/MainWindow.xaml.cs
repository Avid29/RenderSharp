// Adam Dernis 2023

using ComputeSharp;
using Microsoft.UI.Xaml;
using RenderSharp.RayTracing;
using RenderSharp.RayTracing.Models.Materials.Enums;
using RenderSharp.RayTracing.Models.Materials;
using RenderSharp.RayTracing.Shaders.Shading.Stock.MaterialShaders;
using RenderSharp.RayTracing.Shaders.Shading;
using RenderSharp.Rendering;
using RenderSharp.ToneReproduction;
using RenderSharp.UI.Shared.Rendering;
using System.Numerics;
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
        // Create materials
        var color0 = new Vector3(0.9f, 0.2f, 0.1f);
        var material0 = new PhongMaterial(color0, Vector3.One, color0, 80f,
            cDiffuse: 0.8f, cSpecular: 0.9f, cAmbient: 0.2f);
        var color1 = new Vector3(0.25f, 0.35f, 0.35f);
        var material1 = new PhongMaterial(color1, Vector3.One, color1, 10f,
            cDiffuse: 0.8f, cSpecular: 0.9f, cAmbient: 0.2f);
        var yellow = Vector3.UnitX + Vector3.UnitY;
        var material2 = new CheckeredPhongMaterial(yellow, Vector3.UnitX, Vector3.One, 50f, 10f,
            cDiffuse0: 0.8f, cDiffuse1: 0.8f, cSpecular: 0.9f, cAmbient: 0.2f);
        var material3 = new RadialGradientPhongMaterial(Vector3.UnitX, Vector3.UnitY, Vector3.One, 50f, 4f, (int)TextureSpace.Object,
            cDiffuse0: 0.8f, cDiffuse1: 0.8f, cSpecular: 0.9f, cAmbient: 0.2f);
        var material4 = new PrincipledMaterial(Vector3.One * 0.5f, Vector3.One * 0.5f, Vector3.Zero, 10f, 0.8f, 0, 1);
        var material5 = new PrincipledMaterial(new Vector3(0.25f, 0.35f, 0.35f), Vector3.One * 0.5f, Vector3.Zero, 20f, 0.025f, 0, 1);
        var material6 = new PrincipledMaterial(Vector3.One * 0.5f, Vector3.One, Vector3.Zero, 20f, 0f, 0.9f, 0.95f);

        var renderer = new RayTracingRenderer
        {
            Config = new RayTracingConfig
            {
                UseBVH = true
            }
        };

        var postProcessor = new ToneReproducer();

        // Register materials
        renderer.RegisterMaterials<CheckeredPhongShader, CheckeredPhongMaterial>(material2);
        renderer.RegisterMaterials<PrincipledShader, PrincipledMaterial>(material4);
        renderer.RegisterMaterials<PrincipledShader, PrincipledMaterial>(material6);


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
