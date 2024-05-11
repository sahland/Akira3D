namespace Akira.Models.Processing
{
    public class TextureVertex
    {
        public double U { get; set; }
        public double V { get; set; }

        public TextureVertex(double u, double v)
        {
            U = u;
            V = v;
        }
    }
}
