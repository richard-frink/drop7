namespace Drop7.CNN.Layers
{
    public abstract class Layer
    {
        public double[,] Input { get; set; }

        public abstract void Initialize(double[,] board);
        public abstract double[,] CalculateNextLayer();
    }
}
