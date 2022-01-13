using Microsoft.UI.Xaml;
using RenderSharp.RayTracing.GPU;
using RenderSharp.Sample.Shared.Renderer;
using RenderSharp.Scenes;
using RenderSharp.Scenes.Objects.Meshes;
using System.Numerics;

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
            Face face = new Face();
            face.Verticies.Add(Vector3.UnitX);
            face.Verticies.Add(Vector3.UnitY);
            face.Verticies.Add(Vector3.UnitZ);
            Mesh mesh = new Mesh();
            mesh.Faces.Add(face);
            Shader.Scene = Scene.CreateMeshScene(mesh);
        }

        public RenderViewer<HlslRayTraceRenderer> Shader = new RenderViewer<HlslRayTraceRenderer>(new HlslRayTraceRenderer());
    }
}
