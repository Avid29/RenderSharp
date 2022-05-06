using Microsoft.UI.Xaml;
using RenderSharp.Import;
using RenderSharp.RayTracing.GPU;
using RenderSharp.Sample.Shared.Renderer;
using RenderSharp.Scenes;
using RenderSharp.Scenes.Objects.Meshes;
using System.Collections.Generic;

namespace RenderSharp.Sample.WinUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            Setup();
        }

        private void Setup()
        {
            string path = @"C:\temp\CompanionCube.obj";
            List<Mesh> meshes = WaveFrontImporter.Parse(path);

            //Face face = new Face();
            //face.Verticies.Add(Vector3.UnitX);
            //face.Verticies.Add(Vector3.UnitY);
            //face.Verticies.Add(Vector3.UnitZ);
            //Mesh mesh = new Mesh();
            //mesh.Faces.Add(face);
            Shader.Scene = Scene.CreateScene(meshes);
        }

        public RenderViewer<HlslRayTraceRenderer> Shader = new RenderViewer<HlslRayTraceRenderer>(new HlslRayTraceRenderer());
    }
}
