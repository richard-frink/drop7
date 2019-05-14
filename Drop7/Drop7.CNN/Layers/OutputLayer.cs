using System.Collections.Generic;
using System.Linq;

namespace Drop7.CNN.Layers
{
    public class OutputLayer : Layer
    {
        public override void Initialize(double[,] board)
        {
            Input = board;
        }

        public override double[,] CalculateNextLayer()
        {
            var output = new double[1, 1];
            var outputLayer = new Dictionary<int, double>();
            var minimumIndexes = new List<int>();

            for (int i = 0; i < 7; i++)
            {
                outputLayer.Add(i, Input[0, i]);
            }

            double minValue = outputLayer.Values.Min();

            for (int i = 0; i < 7; i++)
            {
                if (Input[0, i] == minValue)
                    minimumIndexes.Add(i);
            }

            // out of every index that had the minimum resul, pick one randomly
            var random = new System.Random();
            output[0,0] = minimumIndexes.ElementAt(random.Next(0, minimumIndexes.Count));

            return output;
        }
    }
}
