namespace Akira.Models.Processing
{
    public class VertexIndex
    {
        public int VertexIndex_X { get; set; }
        public int TextureVertexIndex { get; set; }
        public int NormalIndex { get; set; }

        public VertexIndex(int vertexIndex, int textureVertexIndex, int normalTexture)
        {
            VertexIndex_X = vertexIndex;
            TextureVertexIndex = textureVertexIndex;
            NormalIndex = normalTexture;
        }
    }
}
