using System.Collections.Generic;

namespace Akira.Models.Processing
{
    public class Group
    {
        public string Name { get; set; }
        public List<Face> Faces { get; set; }

        public Group()
        {
            Faces = new List<Face>();
        }
    }
}
