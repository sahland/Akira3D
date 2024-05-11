using Akira.Models.ObjLoader;
using SharpGL;
using SharpGL.SceneGraph.Assets;
using System;

namespace Akira.Models
{
    public class AkiraRender
    {
        private void SetTextureParameters(OpenGL gl) { }

        public void Loading(OpenGL gl, String modelPath, String texturePath, ObjFile objFile, Texture texture)
        {
            SetTextureParameters(gl);
            gl.Enable(OpenGL.GL_TEXTURE_2D);

            try
            {
                gl.PushMatrix();
                gl.Material(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE, new Single[] { 1.0f, 1.0f, 1.0f, 1.0f });


                foreach (var group in objFile.Groups)
                {
                    foreach (var face in group.Faces)
                    {

                        gl.Begin(OpenGL.GL_POLYGON);
                        foreach (var vertexIndex in face.VertexIndices)
                        {
                            var vertex = objFile.Vertices[vertexIndex.VertexIndex_X - 1];
                            var textureCoordinate = objFile.TextureCoordinates[vertexIndex.TextureVertexIndex - 1];

                            gl.TexCoord(textureCoordinate.U, 1.0 - textureCoordinate.V);
                            gl.Vertex(vertex.X, vertex.Y, vertex.Z);
                        }
                        gl.End();
                    }
                }
                gl.PopMatrix();

            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            gl.Disable(OpenGL.GL_TEXTURE_2D);
        }
    }
}
