using Akira.Models.Processing;
using DynamicData;
using FileFormatWavefront.Model;
using GlmNet;
using SharpGL;
using SharpGL.Shaders;
using SharpGL.VertexBuffers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Akira.Models.Auxiliary
{
    public class Scene
    {

        #region Fields
        private readonly Material _defaultMaterial;

        private readonly List<Mesh> _meshes;
        private readonly Dictionary<Mesh, VertexBufferArray> _meshVertexBufferArrays;
        private readonly Dictionary<Mesh, Texture2D> _meshTexture;

        private ShaderProgram _shaderPerPixel;

        private mat4 _modelviewMatrix;
        private mat4 _projectionMatrix;
        private mat3 _normalMatrix;

        private float _scaleFactor;
        #endregion

        public Scene()
        {
            #region Initialise fields
            _meshes = new List<Mesh>();
            _meshVertexBufferArrays = new Dictionary<Mesh, VertexBufferArray>();
            _meshTexture = new Dictionary<Mesh, Texture2D>();

            _modelviewMatrix = mat4.identity();
            _projectionMatrix = mat4.identity();
            _normalMatrix = mat3.identity();

            _scaleFactor = 1.0f;
            #endregion

        }

        public void Initialise(OpenGL gl)
        {
            // Создание попиксельного шейдера
            _shaderPerPixel = new ShaderProgram();
            _shaderPerPixel.Create(
                gl,
                ManifestResourceLoader.LoadTextFile(@"Models\Shaders\PerPixel\PerPixel.vert"),
                ManifestResourceLoader.LoadTextFile(@"Models\Shaders\PerPixel\PerPixel.frag"),
                null
                );
            _shaderPerPixel.BindAttributeLocation(gl, VertexAttributes.Position, "Position");
            _shaderPerPixel.BindAttributeLocation(gl, VertexAttributes.Normal, "Normal");
            gl.ClearColor(0f, 0f, 0f, 1f);

            gl.Enable(OpenGL.GL_TEXTURE_2D);
        }

        // Создание матрицы проекции для данного размера экрана
        public void CreateProjectionMatrix(float screenWidth, float screenHeight)
        {
            const float S = 0.46f;
            float H = S * screenHeight / screenWidth;
            _projectionMatrix = glm.frustum(-S, S, -H, H, 1, 100);
        }

        // Создание модели с возможностью вращения
        public void CreateModelviewAndNormalMatrix(float rotationAngle)
        {
            mat4 rotation = glm.rotate(mat4.identity(), rotationAngle, new vec3(0, 1, 0));
            mat4 translation = glm.translate(mat4.identity(), new vec3(0, 0,  -40));
            mat4 scale = glm.scale(mat4.identity(), new vec3(_scaleFactor, _scaleFactor, _scaleFactor));
            _modelviewMatrix = scale * rotation * translation;
            _normalMatrix = _modelviewMatrix.to_mat3();
        }

        public void RenderImmediateMode(OpenGL gl)
        {
            // Настрока матрицы просмотра модели
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();
            gl.MultMatrix(_modelviewMatrix.to_array());

            // Проход по каждой группе
            foreach(var mesh in _meshes)
            {
                var texture = _meshTexture.ContainsKey(mesh) ? _meshTexture[mesh] : null;
                if(texture == null)
                {
                    texture.Bind(gl);
                }

                uint mode = OpenGL.GL_TRIANGLES;
                if(mesh.indicesPerFace == 4)
                {
                    mode = OpenGL.GL_QUADS;
                }
                else if(mesh.indicesPerFace > 4)
                {
                    mode = OpenGL.GL_POLYGON;
                }

                // Рендер группы фейсов
                gl.Begin(mode);
                for(int i = 0; i < mesh.vertices.Length; i++)
                {
                    gl.Vertex(mesh.vertices[i].x, mesh.vertices[i].y, mesh.vertices[i].z);
                    if(mesh.normals != null)
                    {
                        gl.Normal(mesh.normals[i].x, mesh.normals[i].y, mesh.normals[i].z);
                    }
                    if(mesh.uvs != null)
                    {
                        gl.TexCoord(mesh.uvs[i].x, mesh.uvs[i].y);
                    }
                }
                gl.End();

                if(texture != null)
                {
                    texture.Unbind(gl);
                }
            }
        }

        public void RenderRetainedMode(OpenGL gl)
        {
            // Используем программу шейдера
            _shaderPerPixel.Bind(gl);

            // Задаем позицию света
            _shaderPerPixel.SetUniform3(gl, "LightPosition", 0.25f, 0.25f, 10f);

            // Задаем матрицы
            _shaderPerPixel.SetUniformMatrix4(gl, "Projection", _projectionMatrix.to_array());
            _shaderPerPixel.SetUniformMatrix4(gl, "Modelview", _modelviewMatrix.to_array());
            _shaderPerPixel.SetUniformMatrix4(gl, "NormalMatrix", _normalMatrix.to_array());

            
            foreach(var mesh in _meshes)
            {

                // Если есть материал для сетки, то используем его.
                // Иначе будем использовать материал по умолчанию
                if (mesh.material != null)
                {
                    _shaderPerPixel.SetUniform3(gl, "DiffuseMaterial", mesh.material.Diffuse.r, mesh.material.Diffuse.g, mesh.material.Diffuse.b);
                    _shaderPerPixel.SetUniform3(gl, "AmbientMaterial", mesh.material.Ambient.r, mesh.material.Ambient.g, mesh.material.Ambient.b);
                    _shaderPerPixel.SetUniform3(gl, "SpecularMaterial", mesh.material.Specular.r, mesh.material.Specular.g, mesh.material.Specular.b);
                    _shaderPerPixel.SetUniform1(gl, "Shininess", mesh.material.Shininess);
                }
                else
                {
                    int i = 0;
                }
                var vertexBufferArray = _meshVertexBufferArrays[mesh];
                vertexBufferArray.Bind(gl);

                uint mode = OpenGL.GL_TRIANGLES;
                if(mesh.indicesPerFace == 4)
                {
                    mode = OpenGL.GL_QUADS;
                }
                else if(mesh.indicesPerFace > 4)
                {
                    mode = OpenGL.GL_POLYGON;
                }

                gl.DrawArrays(mode, 0, mesh.vertices.Length);
            }

            // Отвязка шейдера
            _shaderPerPixel.Unbind(gl);
        }

        public void Load(OpenGL gl, string objectFilePath)
        {
            // Очистка старых файлов

            // Удаление всех массивов с вершинами 
            foreach(var vertexBufferArray in _meshVertexBufferArrays.Values)
            {
                vertexBufferArray.Delete(gl);
            }
            _meshes.Clear();
            _meshVertexBufferArrays.Clear();

            // Загрузка .obj файла
            var result = FileFormatWavefront.FileFormatObj.Load(objectFilePath, true);
            //_meshes.AddRange(Denormalizer.Denormalize(result.Model));

            // Создание массива буферов вертексов
            _meshes.ForEach(m => CreateVertexBufferArray(gl, m));

            // Создание текстуры для каждой текстурной сетки
            CreateTextures(gl, _meshes);
        }

        public float SetScaleFactorAuto()
        {
            if (!_meshes.Any())
            {
                _scaleFactor = 1.0f;
                return _scaleFactor;
            }

            var maxX = _meshes.SelectMany(m => m.vertices).AsParallel().Max(v => Math.Abs(v.x));
            var maxY = _meshes.SelectMany(m => m.vertices).AsParallel().Max(v => Math.Abs(v.y));
            var maxZ = _meshes.SelectMany(m => m.vertices).AsParallel().Max(v => Math.Abs(v.z));
            var max = (new[] { maxX, maxY, maxZ }).Max();

            // Устанавливаем коеффицент масштабирования
            _scaleFactor = 8.0f / max;
            return _scaleFactor;
        }

        #region Private methods
        private void CreateVertexBufferArray(OpenGL gl, Mesh mesh)
        {
            // Создание и биндинг массива буферов вертексов
            var vertexBufferArray = new VertexBufferArray();
            vertexBufferArray.Create(gl);
            vertexBufferArray.Bind(gl);

            // Создание буфера вершин для вертексов
            var verticesVertexBuffer = new VertexBuffer();
            verticesVertexBuffer.Create(gl);
            verticesVertexBuffer.Bind(gl);
            verticesVertexBuffer.SetData(gl, VertexAttributes.Position,
                                mesh.vertices.SelectMany(v => v.to_array()).ToArray(),
                                false, 3);

            if(mesh.normals != null)
            {
                var normalsVertexBuffer = new VertexBuffer();
                normalsVertexBuffer.Create(gl);
                normalsVertexBuffer.Bind(gl);
                normalsVertexBuffer.SetData(gl, VertexAttributes.Normal,
                                    mesh.normals.SelectMany(v => v.to_array()).ToArray(),
                                    false, 3);
            }

            if(mesh.uvs != null)
            {
                var texCoordsVertexBuffer = new VertexBuffer();
                texCoordsVertexBuffer.Create(gl);
                texCoordsVertexBuffer.Bind(gl);
                texCoordsVertexBuffer.SetData(gl, VertexAttributes.TexCoord,
                                    mesh.uvs.SelectMany(v => v.to_array()).ToArray(),
                                    false, 3);
            }

            verticesVertexBuffer.Unbind(gl);
            _meshVertexBufferArrays[mesh] = vertexBufferArray;
        }

        private void CreateTextures(OpenGL gl, IEnumerable<Mesh> meshes)
        {
            foreach(var mesh in _meshes.Where(m => m.material != null && m.material.TextureMapDiffuse != null))
            {
                // Создание новой текстуры и её биндинг
                var texture = new Texture2D();
                texture.Create(gl);
                texture.Bind(gl);
                texture.SetPatemeter(gl, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
                texture.SetPatemeter(gl, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);
                texture.SetPatemeter(gl, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_CLAMP_TO_EDGE);
                texture.SetPatemeter(gl, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_CLAMP_TO_EDGE);
                texture.SetImage(gl, (Bitmap)mesh.material.TextureMapDiffuse.Image);
                texture.Unbind(gl);
                _meshTexture[mesh] = texture;
            }
        }
        #endregion

        #region Properties
        public float ScaleFactor
        {
            get { return _scaleFactor; }
            set { _scaleFactor = value; }
        }

        public mat4 ProjectionMatrix
        {
            get { return _projectionMatrix; }
        }
        #endregion
    }
}
