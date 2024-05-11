using System.Collections.Generic;

namespace Akira.Models.Processing
{
    public class Model
    {
        public List<Vertex> Vertices { get; set; }
        public List<Normal> Normals { get; set; }
        public List<TextureVertex> TextureVertices { get; set; }
        public List<Face> Faces { get; set; }

        public Model(List<Vertex> Vertices, List<Normal> Normals, List<TextureVertex> TextureVertices, List<Face> Faces)
        {
            this.Vertices = Vertices;
            this.Normals = Normals;
            this.TextureVertices = TextureVertices;
            this.Faces = Faces;
        }
    }
}
