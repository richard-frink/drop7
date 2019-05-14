using System;
using System.Collections.Generic;
using System.Text;

namespace Drop7.CNN.Layers
{
    public class PoolingLayer : Layer
    {
        public override void Initialize(double[,] board)
        {
            Input = board;
        }

        public override double[,] CalculateNextLayer()
        {
            var output = new double[1, 9];

            int baseColumn = 0;
            for (int c = 0; c < 9; c++)
            {
                int extraOdd = 0;
                // odd column
                if (c % 2 == 1)
                {
                    baseColumn++;
                    extraOdd = -1;
                }

                double columnSum = 0;
                int left = baseColumn + extraOdd;
                int right = baseColumn;

                for (int r = 0; r < 4; r++)
                {
                    columnSum += Math.Floor((Input[r,left]+Input[r,right]) / 2);
                }

                output[0, c] = columnSum;
            }

            return output;
        }
    }
}
