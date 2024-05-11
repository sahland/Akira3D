using GlmNet;
using FileFormatWavefront.Model;

namespace Akira.Models.Processing
{
    public class Mesh
    {
        public vec3[] vertices;
        public vec3[] normals;
        public vec2[] uvs;
        public uint[] indices;
        public int indicesPerFace;
        public Material material;
    }
}
