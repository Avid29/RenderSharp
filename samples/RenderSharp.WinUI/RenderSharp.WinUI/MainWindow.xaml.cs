using ComputeSharp.WinUI;
using Microsoft.UI.Xaml;
using RenderSharp.Common.Scenes;
using RenderSharp.Common.Scenes.Objects.Meshes;
using RenderSharp.Import;
using RenderSharp.RayTracing.CPU;
using RenderSharp.RayTracing.HLSL;
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

            InitScene();
        }

        public void InitScene()
        {
            // TODO: File Picker
            //FileOpenPicker picker = new FileOpenPicker() { CommitButtonText = "Select", SuggestedStartLocation = PickerLocationId.Objects3D, FileTypeFilter = { ".obj" } };
            //var file = await picker.PickSingleFileAsync();
            //string path = file.Path;
            string path = @"C:\Users\avid2\3D Objects\TriMonkey.obj";

            Mesh mesh = WaveFrontImporter.LoadMesh(path);
            Shader.Scene = Scene.CreateMeshScene(mesh);
            Shader.Setup(new HlslRayTraceRenderer());
        }

        public RenderViewer<HlslRayTraceRenderer> Shader = new RenderViewer<HlslRayTraceRenderer>();
        //public ISceneRenderer Shader = new ProgressiveRenderer<CPURenderer>(new CPURenderer());
    }
}
