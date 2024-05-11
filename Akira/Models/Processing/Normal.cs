namespace Akira.Models.Processing
{
    public class Normal
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Normal(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}
