using SharpGL;
using SharpGL.SceneGraph.Assets;
using System;

namespace Akira.Models.ObjLoader
{
    public class ModelHandler
    {
        private ObjFile _objFile;
        private Texture _texture;
        //private Int32 _count;

        public void Loading(OpenGL gl, String modelPath, String texturePath)
        {
            try
            {
                gl.Enable(OpenGL.GL_TEXTURE_2D);

                _objFile = new ObjFile();
                _objFile.Load(modelPath);

                gl.Material(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE, new Single[] { 1.0f, 1.0f, 1.0f, 1.0f });

                gl.PushMatrix();

                foreach (var group in _objFile.Groups)
                {
                    foreach (var face in group.Faces)
                    {
                        gl.Begin(OpenGL.GL_POLYGON);
                        foreach (var vertexIndex in face.VertexIndices)
                        {
                            var vertex = _objFile.Vertices[vertexIndex.VertexIndex_X - 1];
                            var textureCoordinate = _objFile.TextureCoordinates[vertexIndex.TextureVertexIndex - 1];

                            gl.TexCoord(textureCoordinate.U, 1.0 - textureCoordinate.V);
                            gl.Vertex(vertex.X, vertex.Y, vertex.Z);
                        }
                        gl.End();
                    }
                }

                _texture.Destroy(gl);
                gl.PopMatrix();
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
