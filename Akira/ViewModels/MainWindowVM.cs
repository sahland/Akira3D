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
        private static OpenGL _gl;
        private static ObjFile _objFile;
        private static Texture _texture;
        private ModelRotator _modelRotator;

        private float rotate;
        private float _theta;
        private float _actWidth;
        private float _actHeight;
        private string _modelPath;
        private string _texturePath;
        private static bool _loaded;

        public DelegateCommand OpenFileCommand { get; private set; }
        public DelegateCommand ResizeCommand { get; private set; }
        public DelegateCommand DrawCommand { get; private set; }
        public DelegateCommand InitializedCommand { get; private set; }

        public MainWindowVM()
        {
            _openFileDialog = new Microsoft.Win32.OpenFileDialog();
            _akiraRender = new AkiraRender();
            _gl = new OpenGL();
            _axies = new Axies();
            _scene = new Scene();
            _modelRotator = new ModelRotator(_gl);

            _theta = 0;
            rotate = 10f;
            _loaded = false;

            OpenFileCommand = new DelegateCommand(OpenFile);
            InitializedCommand = new DelegateCommand(Initialized);
            ResizeCommand = new DelegateCommand(Resized);
            DrawCommand = new DelegateCommand(Draw);
        }

        public void Draw()
        {
            _theta += 0.01f;
            _scene.CreateModelviewAndNormalMatrix(_theta);

            _gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            _gl.ClearColor(0, 0, 0, 0);
            _gl.LoadIdentity();
            _gl.Translate(-1.0f, -1.0f, -12.0f);

            _axies.Render(_gl, RenderMode.Design);

            //_gl.Rotate(_modelRotator.AngleX, 1.0f, 0.0f, 0.0f);
            //_gl.Rotate(_modelRotator.AngleY, 0.0f, 1.0f, 0.0f);
            //_gl.Rotate(_modelRotator.AngleZ, 0.0f, 0.0f, 1.0f);
            _gl.Rotate(rotate++, 0.0f, 1.0f, 0.0f);

            //if(loaded)
            //{
            //    LoadAndRenderModel();
            //}

            LoadAndRenderModel();
        }

        private void Initialized()
        {

            _scene.Initialise(_gl);

            _gl.Enable(OpenGL.GL_DEPTH_TEST);
        }

        private void Resized()
        {

            _scene.CreateProjectionMatrix(_actWidth, _actHeight);

            _gl.MatrixMode(OpenGL.GL_PROJECTION);
            _gl.LoadIdentity();
            _gl.MultMatrix(_scene.ProjectionMatrix.to_array());
            _gl.MatrixMode(OpenGL.GL_MODELVIEW);
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
                _loaded = true;
            }
        }

        private void LoadAndRenderModel()
        {
            AkiraRender ar = new AkiraRender();

            Console.WriteLine(_modelPath);
            Console.WriteLine(_texturePath);

            if (_objFile == null)
            {
                _objFile = new ObjFile();

                //objFile.Load(_modelPath);
                _objFile.Load("ar-15_lp.obj");
            }

            if (_texture == null)
            {
                _gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
                //         gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);
                _gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_REPEAT);
                _gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_REPEAT);
                // Загрузка текстуры
                _texture = new Texture();
                //texture.Create(gl, _texturePath);
                _texture.Create(_gl, "ar-15_lp.png");
            }

            ar.Loading(_gl, "ar-15_lp.obj", "ar-15_lp.png", _objFile, _texture);

        }

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
