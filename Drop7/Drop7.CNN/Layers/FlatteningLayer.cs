using System.Collections.Generic;
using System.Linq;

namespace Drop7.CNN.Layers
{
    public class FlatteningLayer : Layer
    {
        private Tiles[,] board { get; set; }

        public FlatteningLayer(Tiles[,] board)
        {
            this.board = board;
        }

        public override void Initialize(double[,] board)
        {
            Input = board;
        }

        public override double[,] CalculateNextLayer()
        {
            var output = new double[1, 7];
            var ineligibleColumns = GetIneligibleColumns();

            var poolingLayerOutput = new Dictionary<int, double>();
            for (int i = 0; i < 9; i++)
            {
                poolingLayerOutput.Add(i, Input[0, i]);
            }

            double minValue = poolingLayerOutput.Values.Min();
            double maxValue = poolingLayerOutput.Values.Max();

            int minColumn = poolingLayerOutput.First(item => item.Value == minValue).Key;
            int maxColumn = poolingLayerOutput.First(item => item.Value == maxValue).Key;

            int baseColumn = 0;
            for (int c = 0; c < 9; c++)
            {
                // these could happen back to back so we have to check twice
                if (c == minColumn || c == maxColumn)
                    c++;
                if (c == minColumn || c == maxColumn)
                    c++;

                if (c > 8)
                    continue;

                output[0, baseColumn] = Input[0, c];

                // unfortunately, this can favor full columns, so we have to filter those by driving the score to be extremely undesireable
                if (ineligibleColumns.Count > 0)
                    if (ineligibleColumns.Contains(baseColumn))
                        output[0, baseColumn] += 10000;

                baseColumn++;
            }

            return output;
        }

        private List<double> GetIneligibleColumns()
        {
            var ineligible = new List<double>();

            for (int column = 0; column < 7; column++)
            {
                // if not empty at top of column
                if ((int)board[0, column] != 10)
                    ineligible.Add(column);
            }

            return ineligible;
        }
    }
}
