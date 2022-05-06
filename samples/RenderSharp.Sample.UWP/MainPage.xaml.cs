using Windows.UI.Xaml.Controls;
using RenderSharp.RayTracing.CPU;
using RenderSharp.RayTracing.GPU;
using RenderSharp.Sample.Shared.Renderer;
using RenderSharp.Scenes;
using RenderSharp.Scenes.Objects.Meshes;
using System.Numerics;
using RenderSharp.Scenes.Materials;
using System.Collections.Generic;

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
            face.Verticies.Add(Vector3.Zero);
            Mesh mesh = new Mesh();
            mesh.Faces.Add(face);
            mesh.Material = new DiffuseMaterial(new Vector4(1f, 0f, 0f, 1f), .5f);
            Face face2 = new Face();
            face2.Verticies.Add(Vector3.UnitX);
            face2.Verticies.Add(Vector3.UnitZ);
            face2.Verticies.Add(Vector3.Zero);
            Mesh mesh2 = new Mesh();
            mesh2.Faces.Add(face2);
            mesh2.Material = new DiffuseMaterial(new Vector4(0f, 0f, 1f, 1f), .5f);
            Shader.Scene = Scene.CreateScene(new List<Mesh>() { mesh, mesh2 });
        }

        public RenderViewer<HlslRayTraceRenderer> Shader = new RenderViewer<HlslRayTraceRenderer>(new HlslRayTraceRenderer());
        //public RenderViewer<CPURayTraceRenderer> Shader = new RenderViewer<CPURayTraceRenderer>(new CPURayTraceRenderer());
    }
}
