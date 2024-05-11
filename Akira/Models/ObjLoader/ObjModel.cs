using Akira.Models.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Akira.Models.ObjLoader
{
    public class ObjModel
    {
        public List<Vertex> vertices { get; set; }
        public List<TextureVertex> textureVertices { get; set; }
        public List<Normal> normals { get; set; }
        public List<Face> faces { get; set; }

        public ObjModel()
        {
            vertices = new List<Vertex>();
            textureVertices = new List<TextureVertex>();
            normals = new List<Normal>();
            faces = new List<Face>();
        }

        public void Load(String fileName)
        {
            StreamReader reader = new StreamReader(fileName);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                String[] tokens = line.Split(' ');
                switch (tokens[0])
                {
                    case "v":
                        vertices.Add(new Vertex(Single.Parse(tokens[1]), Single.Parse(tokens[2]), Single.Parse(tokens[3])));
                        break;
                    case "vt":
                        textureVertices.Add(new TextureVertex(float.Parse(tokens[1]), float.Parse(tokens[2])));
                        break;
                    case "vn":
                        normals.Add(new Normal(float.Parse(tokens[1]), float.Parse(tokens[2]), float.Parse(tokens[3])));
                        break;
                    case "f":
                        Face face = new Face();
                        for (int i = 1; i < tokens.Length; i++)
                        {
                            string[] indices = tokens[i].Split('/');
                            face.VertexIndices.Add(new VertexIndex(int.Parse(indices[0]), int.Parse(indices[1]), int.Parse(indices[2])));
                        }
                        faces.Add(face);
                        break;
                    default:
                        break;
                }
            }
            reader.Close();
        }
    }
}
