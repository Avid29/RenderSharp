using Microsoft.UI.Xaml;
using RenderSharp.Common.Scenes;
using RenderSharp.Common.Scenes.Materials;
using RenderSharp.Common.Scenes.Objects.Meshes;
using RenderSharp.Import.WaveFront;
using RenderSharp.RayTracing.HLSL;
using RenderSharp.WinUI.Renderer;
using System.Numerics;
using System.Collections.Generic;
using RenderSharp.Common.Scenes.Objects;
using RenderSharp.RayTracing.CPU;

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
            string path = @"C:\Users\avid2\3D Objects\TriCube-Emission.obj";
            WaveFrontImporter importer = new WaveFrontImporter(path);

            Shader.Scene = Scene.CreateMeshScene(importer.Objects);
            Shader.Setup(new HlslRayTraceRenderer());
            //Shader.Setup(new CPURayTraceRenderer());
        }

        public RenderViewer<HlslRayTraceRenderer> Shader = new RenderViewer<HlslRayTraceRenderer>();
        //public RenderViewer<CPURayTraceRenderer> Shader = new RenderViewer<CPURayTraceRenderer>();
    }
}
