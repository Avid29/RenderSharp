using Windows.UI.Xaml.Controls;
using RenderSharp.RayTracing.CPU;
using RenderSharp.RayTracing.GPU;
using RenderSharp.Sample.Shared.Renderer;
using RenderSharp.Scenes;
using RenderSharp.Scenes.Objects.Meshes;
using System.Numerics;

namespace RenderSharp.Sample.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
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
        //public RenderViewer<CPURayTraceRenderer> Shader = new RenderViewer<CPURayTraceRenderer>(new CPURayTraceRenderer());
    }
}
