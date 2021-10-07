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
            string path = @"C:\Users\avid2\3D Objects\CompanionCubeSmall-Mat.obj";
            WaveFrontImporter importer = new WaveFrontImporter(path);

            //List<IObject> objects = new List<IObject>();

            // Add plane
            SuperMaterial material = new SuperMaterial();
            material.Roughness = 0.2f;
            material.Metallic = 1f;
            material.Albedo = new Vector4(0.6f, 0.6f, 0.6f, 1f);
            material.Emission = Vector4.Zero;

            //SuperMaterial emissive = new SuperMaterial();
            //emissive.Roughness = 0.5f;
            //emissive.Metallic = 0.0f;
            //emissive.Albedo = new Vector4(1, 0f, 0f, 1f);
            //emissive.Emission = new Vector4(1, 0f, 0f, 1f);

            //Sphere sphere = new Sphere(new Vector3(0), .5f, emissive);
            //objects.Add(sphere);

            Mesh plane = new Mesh();
            Vector3 corner1 = new Vector3(-10, -0.25f, -10);
            Vector3 corner2 = new Vector3(10, -0.25f, -10);
            Vector3 corner3 = new Vector3(-10, -0.25f, 10);
            Vector3 corner4 = new Vector3(10, -0.25f, 10);
            plane.Verticies.Add(corner1);
            plane.Verticies.Add(corner2);
            plane.Verticies.Add(corner3);
            plane.Verticies.Add(corner4);
            Face face1 = new Face();
            face1.Verticies.Add(corner2);
            face1.Verticies.Add(corner1);
            face1.Verticies.Add(corner3);
            Face face2 = new Face();
            face2.Verticies.Add(corner2);
            face2.Verticies.Add(corner3);
            face2.Verticies.Add(corner4);
            plane.Faces.Add(face1);
            plane.Faces.Add(face2);
            plane.Material = material;
            importer.Objects.Add(plane);

            Shader.Scene = Scene.CreateMeshScene(importer.Objects);
            Shader.Setup(new HlslRayTraceRenderer());
            //Shader.Setup(new CPURayTraceRenderer());
        }

        public RenderViewer<HlslRayTraceRenderer> Shader = new RenderViewer<HlslRayTraceRenderer>();
        //public RenderViewer<CPURayTraceRenderer> Shader = new RenderViewer<CPURayTraceRenderer>();
    }
}
