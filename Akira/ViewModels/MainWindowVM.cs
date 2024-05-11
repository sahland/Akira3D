using Akira.Models;
using Akira.Models.Auxiliary;
using Akira.Models.ObjLoader;
using Prism.Commands;
using Prism.Mvvm;
using SharpGL;
using SharpGL.SceneGraph.Assets;
using SharpGL.SceneGraph.Core;
using SharpGL.SceneGraph.Primitives;
using System;

namespace Akira.ViewModels
{
    public class MainWindowVM : BindableBase
    {

        private readonly Axies _axies;
        private readonly Scene _scene;
        private Microsoft.Win32.OpenFileDialog _openFileDialog;
        private AkiraRender _akiraRender;
        private static OpenGL gl;
        private float rotate = 10f;

        private float _theta;
        private float _actWidth;
        private float _actHeight;
        private string _modelPath;
        private string _texturePath;

        public DelegateCommand OpenFileCommand { get; private set; }
        public DelegateCommand ResizeCommand { get; private set; }
        public DelegateCommand DrawCommand { get; private set; }
        public DelegateCommand InitializedCommand { get; private set; }

        public MainWindowVM()
        {
            _openFileDialog = new Microsoft.Win32.OpenFileDialog();
            _akiraRender = new AkiraRender();
            gl = new OpenGL();

            _axies = new Axies();
            _scene = new Scene();
            _theta = 0;

            OpenFileCommand = new DelegateCommand(OpenFile);
            InitializedCommand = new DelegateCommand(Initialized);
            ResizeCommand = new DelegateCommand(Resized);
            DrawCommand = new DelegateCommand(Draw);
        }

        public void Draw()
        {
            _theta += 0.01f;
            _scene.CreateModelviewAndNormalMatrix(_theta);

            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.ClearColor(0, 0, 0, 0);
            gl.LoadIdentity();
            gl.Translate(0.0f, -1.0f, -12.0f);

            _axies.Render(gl, RenderMode.Design);
            gl.Rotate(rotate++, 0.0f, 1.0f, 0.0f);

            //if(loaded)
            //{
            //    LoadAndRenderModel();
            //}

            LoadAndRenderModel();
        }

        private void Initialized()
        {

            _scene.Initialise(gl);

            gl.Enable(OpenGL.GL_DEPTH_TEST);
        }

        private void Resized()
        {

            _scene.CreateProjectionMatrix(_actWidth, _actHeight);

            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.MultMatrix(_scene.ProjectionMatrix.to_array());
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }

        private void OpenFile()
        {
            _openFileDialog.Filter = "Wavefront Files (*.obj)|*.obj|All Files (*.*)|*.*";
            _openFileDialog.Multiselect = false;

            if (_openFileDialog.ShowDialog() == true)
            {
                _modelPath = _openFileDialog.FileName;
                _texturePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(_modelPath), System.IO.Path.GetFileNameWithoutExtension(_modelPath) + ".png");

                //LoadAndRenderModel();
                loaded = true;
            }
        }

        private void LoadAndRenderModel()
        {
            /*
            // Use your ObjFile class to load the model
            var objFile = new ObjFile();
            objFile.Load(_modelPath);

            // Load the texture
            var texture = new Texture();
            using (var image = new Bitmap(_texturePath))
            {
                texture.Create(_args.OpenGL, image);  // Use 'gl' from 'args'
            }

            // Call AkiraRender to render the model with the texture
            _akiraRender.Loading(_args.OpenGL, _modelPath, _texturePath, objFile, texture);
            */
            AkiraRender ar = new AkiraRender();

            Console.WriteLine(_modelPath);
            Console.WriteLine(_texturePath);

            if (objFile == null)
            {
                objFile = new ObjFile();

                //objFile.Load(_modelPath);
                objFile.Load("ar-15_lp.obj");
            }

            if (texture == null)
            {
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
                //         gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_REPEAT);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_REPEAT);
                // Загрузка текстуры
                texture = new Texture();
                //texture.Create(gl, _texturePath);
                texture.Create(gl, "ar-15_lp.png");
            }

            ar.Loading(gl, "ar-15_lp.obj", "ar-15_lp.png", objFile, texture);

        }

        private static ObjFile objFile;
        private static Texture texture;
        private static bool loaded = false;

        public float actWidth
        {
            get { return _actWidth; }

            set
            {
                if (_actWidth >= 0)
                    _actWidth = value;
            }
        }

        public float actHeight
        {
            get { return _actHeight; }

            set
            {
                if (_actHeight >= 0)
                    _actHeight = value;
            }
        }
    }
}
