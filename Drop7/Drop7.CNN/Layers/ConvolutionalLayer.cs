using System;
using System.Linq;
using System.Collections.Generic;

namespace Drop7.CNN.Layers
{
    public class ConvolutionalLayer : Layer
    {
        public override void Initialize(double[,] board)
        {
            Input = board;
        }

        public override double[,] CalculateNextLayer()
        {
            var f1 = CalcultateFilter1();
            var f2 = CalcultateFilter2();
            var f3 = CalcultateFilter3();
            var f4 = CalcultateFilter4();

            return CalculateOutput(f1, f2, f3, f4);
        }

        private double[,] CalculateOutput(double[,] f1, double[,] f2, double[,] f3, double[,] f4)
        {
            var f12 = CalcultateFilter12(f1, f2);

            var f123 = CalcultateFilter123(f12, f3);

            return CalcultateFilter1234(f123, f4);
        }

        private double[,] CalcultateFilter1()
        {
            var f1 = new double[4, 5];

            int fRow = 0;
            int fColumn = 0;

            // array index (0-6, not 1-7)
            for (int r = 1; r < 5; r++)
            {
                for (int c = 1; c < 6; c++)
                {
                    // starting at top leftmost index
                    var tiles = GetFrame(Input, r - 1, c - 1, 3, 4);

                    f1[fRow, fColumn] = tiles.Count(t => t != 10);

                    fColumn++;
                }
                fColumn = 0;
                fRow++;
            }

            return f1;
        }

        private double[,] CalcultateFilter2()
        {
            var f2 = new double[4, 5];


            int fRow = 0;
            int fColumn = 0;

            // array index (0-6, not 1-7)
            for (int r = 1; r < 5; r++)
            {
                for (int c = 1; c < 6; c++)
                {
                    // starting at top leftmost index
                    var tiles = GetFrame(Input, r - 1, c - 1, 3, 4);

                    f2[fRow, fColumn] = tiles.Count(t => t == 8 || t == 9);

                    fColumn++;
                }
                fColumn = 0;
                fRow++;
            }

            return f2;
        }

        private double[,] CalcultateFilter3()
        {
            var f3 = new double[3, 5];

            int fRow = 0;
            int fColumn = 0;

            // array index (0-6, not 1-7)
            for (int r = 1; r < 4; r++)
            {
                for (int c = 1; c < 6; c++)
                {
                    // starting at top leftmost index
                    var tiles = GetFrame(Input, r - 1, c - 1, 3, 5);

                    tiles = tiles.Where(t => t > 0 && t < 8)?.ToList();
                    if (!tiles.Any())
                    {
                        f3[fRow, fColumn] = 0;
                    }
                    else
                    {
                        double max = tiles.Max();
                        double min = tiles.Min();

                        f3[fRow, fColumn] = Math.Abs(max - min);
                    }

                    fColumn++;
                }
                fColumn = 0;
                fRow++;
            }

            return f3;
        }

        private double[,] CalcultateFilter4()
        {
            var f4 = new double[5, 5];

            int fRow = 0;
            int fColumn = 0;

            // array index (0-6, not 1-7)
            for (int r = 1; r < 6; r++)
            {
                for (int c = 1; c < 6; c++)
                {
                    // starting at top leftmost index
                    var tiles = GetFrame(Input, r - 1, c - 1, 3, 3);

                    tiles = tiles.Where(t => t > 0 && t < 8)?.ToList();
                    if (!tiles.Any())
                        f4[fRow, fColumn] = 0;
                    else
                        f4[fRow, fColumn] = tiles.Distinct().Count();

                    fColumn++;
                }
                fColumn = 0;
                fRow++;
            }

            return f4;
        }

        private double[,] CalcultateFilter12(double[,] f1, double[,] f2)
        {
            var f12 = new double[4, 5];

            // array index (0-6, not 1-7)
            for (int r = 0; r < 4; r++)
            {
                for (int c = 0; c < 5; c++)
                {
                    f12[r, c] = Math.Abs(f1[r, c] - f2[r, c]);
                }
            }

            return f12;
        }

        // interleave f3 into f12 -> f123[r,c] = f12[r,c] + abs(f3[r,c]above - f3[r,c]below)
        private double[,] CalcultateFilter123(double[,] f12, double[,] f3)
        {
            var f123 = new double[4, 5];

            // array index (0-6, not 1-7)
            for (int r = 0; r < 4; r++)
            {
                for (int c = 0; c < 5; c++)
                {
                    // f3 has 3 rows
                    double above = 0;
                    double below = 0;

                    // no f3 above row 0
                    if (r != 0)
                    {
                        above = f3[r - 1, c];
                    }

                    // no f3 below row 4
                    if (r != 3)
                    {
                        below = f3[r, c];
                    }

                    f123[r, c] = f12[r,c] + Math.Abs(above - below);
                }
            }

            return f123;
        }

        private double[,] CalcultateFilter1234(double[,] f123, double[,] f4)
        {
            var f1234 = new double[4, 5];

            // array index (0-6, not 1-7)
            for (int r = 0; r < 4; r++)
            {
                for (int c = 0; c < 5; c++)
                {
                    // f4 has 5 rows
                    double above = f4[r, c];
                    double below = f4[r + 1, c];

                    f1234[r, c] = f123[r, c]*Math.Max(above,below);
                }
            }

            return f1234;
        }

        // row/column of starting index (top leftmost index), and width and height of scanning frame
        private List<double> GetFrame(double[,] input, int row, int column, int width, int height)
        {
            var frame = new List<double>();

            for (int r = row; r < row + height; r++)
            {
                for (int c = column; c < column + width; c++)
                {
                    frame.Add(input[r, c]);
                }
            }

            return frame;
        }
    }
}
