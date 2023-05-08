// Adam Dernis 2023

using Microsoft.UI.Xaml;
using RenderSharp.ImportExport.WaveFront;
using RenderSharp.RayTracing;
using RenderSharp.RayTracing.Models.Materials;
using RenderSharp.RayTracing.Shaders.Shading.Stock.MaterialShaders;
using RenderSharp.Rendering.Interfaces;
using RenderSharp.Rendering.Manager;
using RenderSharp.Scenes;
using RenderSharp.Scenes.Cameras;
using RenderSharp.Scenes.Geometry;
using RenderSharp.Scenes.Lights;
using RenderSharp.ToneReproduction;
using RenderSharp.UI.Shared.Rendering;
using System.Linq;
using System.Numerics;

namespace RenderSharp.Samples.WinUI;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    const string projectPath = @"C:\Users\avid2\source\repos\Personal\RenderSharp\samples\RenderSharp.Samples.WinUI\";

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
        var renderer = new RayTracingRenderer
        {
            Config = new RayTracingConfig
            {
                UseBVH = true,
                SampleCount = 1,
                MaxBounceDepth = 6,
            }
        };
        RegisterMaterials(renderer);
        var scene = CreateScene();
        var postProcessor = new ToneReproducer();
        //IPostProcessor? postProcessor = null;
        RenderViewer.Setup<TiledRenderManager>(renderer, scene, postProcessor);
    }

    private void AnimatedComputeShaderPanel_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        // Skip refresh if initial size change
        if (e.PreviousSize.Width == 0)
            return;

        RenderViewer.Refresh();
    }

    private static Scene CreateScene()
    {
        #region Standard Scene
#if true
        var import = WaveFrontImporter.Parse($"{projectPath}Assets\\Scene-FullSphere.obj");
        var camera = Camera.CreateFromEuler(new Vector3(0f, 1f, 0f), new Vector3(0f, 180f, 0f), 75);
#endif
        #endregion

        #region Bunny
#if false
        var import = WaveFrontImporter.Parse($"{projectPath}Assets\\Bunny.obj");
        var camera = Camera.CreateFromEuler(new Vector3(0f, 1f, 0f), new Vector3(0f, 180f, 0f), 75);
#endif
        #endregion

        #region Chess Set
#if false
        var import = WaveFrontImporter.Parse($"{projectPath}Assets\\ChessSet.obj");
        var camera = Camera.CreateFromEuler(new Vector3(5f, 3f, -2f), new Vector3(-40f, 110f, 0f), 75);
#endif
        #endregion

        var scene = new Scene(camera);

        scene.Geometry.AddRange(import.Objects.OfType<GeometryObject>());
        scene.Lights.AddRange(new LightSource[]
        {
            new PointLight
            {
                Color = Vector3.One,
                Power = 0.5f,
                Radius = 0.25f,
                Transformation = Transformation.CreateFromTranslation(new Vector3(0.5f, 3.5f, 0.5f)),
            },
            //new PointLight
            //{
            //    Color = Vector3.One,
            //    Power = 0.2f,
            //    Radius = 0.25f,
            //    Transformation = Transformation.CreateFromTranslation(new Vector3(-1.5f, 3.5f, 1f)),
            //},
        });

        return scene;
    }

    private static void RegisterMaterials(RayTracingRenderer renderer)
    {
        // Create materials
        var yellow = Vector3.UnitX + Vector3.UnitY;
        var checkers = new CheckeredPhongMaterial(yellow, Vector3.UnitX, Vector3.One, 50f, 10f,
            cDiffuse0: 0.8f, cDiffuse1: 0.8f, cSpecular: 0.9f, cAmbient: 0.2f);
        var glossy = new PrincipledMaterial(Vector3.One * 0.5f, Vector3.One * 0.5f, Vector3.Zero, 10f, 0.8f, 0, 1);
        var transmissive = new PrincipledMaterial(Vector3.One * 0.5f, Vector3.One, Vector3.Zero, 20f, 0f, 0.9f, 0.95f);

        var white = new PhongMaterial(Vector3.One, Vector3.One, Vector3.One, 50f, 0.95f, 0.95f, 0.1f);
        var black = new PhongMaterial(Vector3.One, Vector3.One, Vector3.One, 20f, 0.1f, 0.95f, 0.01f);
        var glossyBlack = new PrincipledMaterial(0.1f * Vector3.One, 0.95f * Vector3.One, Vector3.Zero, 50f, 0.3f, 0, 1);

        // Register materials
        #region Standard Scene
#if true
        renderer.RegisterMaterials<CheckeredPhongShader, CheckeredPhongMaterial>(checkers);
        renderer.RegisterMaterials<PrincipledShader, PrincipledMaterial>(glossy);
        renderer.RegisterMaterials<PrincipledShader, PrincipledMaterial>(transmissive);
#endif
        #endregion

        #region Bunny
#if false
        renderer.RegisterMaterials<PhongShader, PhongMaterial>(white);
#endif
        #endregion

        #region ChessSet
#if false
        // Black pieces
        for (int i = 0; i < 16; i++)
            renderer.RegisterMaterials<PhongShader, PhongMaterial>(black);

        // White pieces
        for (int i = 0; i < 16; i++)
            renderer.RegisterMaterials<PhongShader, PhongMaterial>(white);

        // Black Tiles
        for (int i = 0; i < 32; i++)
            renderer.RegisterMaterials<PrincipledShader, PrincipledMaterial>(glossyBlack);

        // White Tiles
        for (int i = 0; i < 32; i++)
            renderer.RegisterMaterials<PhongShader, PhongMaterial>(white);
#endif
        #endregion
    }
}
