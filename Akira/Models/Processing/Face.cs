using System.Collections.Generic;

namespace Akira.Models.Processing
{
    public class Face
    {
        public string MaterialLibrary { get; set; }
        public string GroupName { get; set; }
        public List<VertexIndex> VertexIndices { get; private set; } = new List<VertexIndex>();
    }
}
