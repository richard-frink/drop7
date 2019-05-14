using Drop7.CNN.Layers;
using System.Collections.Generic;

namespace Drop7.CNN
{
    public class ConvolutionalNeuralNetworkHelper
    {
        private Queue<Layer> layers;

        public void InitializeLayers(Tiles[,] board)
        {
            layers = new Queue<Layer>();
            layers.Enqueue(new ConvolutionalLayer());
            layers.Enqueue(new PoolingLayer());
            layers.Enqueue(new FlatteningLayer(board));
            layers.Enqueue(new OutputLayer());
        }

        public int GetNextResult(Tiles[,] board, int nextTile)
        {
            // reset layers if we have used helper before
            InitializeLayers(board);

            var convertedBoard = ConvertBoardFromTiles(board);

            // convolutional layer
            // create 4 filters and combine in a meaningful way
            var convolutionalLayer = layers.Dequeue();
            convolutionalLayer.Initialize(convertedBoard);
            var convolutionalLayerOutput = convolutionalLayer.CalculateNextLayer();

            // pooling layer
            // aggregate convolutional layer into 9 columns that smooths out the results
            var poolingLayer = layers.Dequeue();
            poolingLayer.Initialize(convolutionalLayerOutput);
            var poolingLayerOutput = poolingLayer.CalculateNextLayer();

            // flattening layer
            // remove the smallest and largest values from our flattened layer
            var flatteningLayer = layers.Dequeue();
            flatteningLayer.Initialize(poolingLayerOutput);
            var flatteningLayerOutput = flatteningLayer.CalculateNextLayer();

            // output layer
            // get the smallest result
            var outputLayer = layers.Dequeue();
            outputLayer.Initialize(flatteningLayerOutput);
            var outputLayerOutput = outputLayer.CalculateNextLayer();

            // change from 0-based to 1-based index
            return (int)outputLayerOutput[0, 0] + 1;
        }

        private double[,] ConvertBoardFromTiles(Tiles[,] board)
        {
            var convertedBoard = new double[7, 7];

            for (int r = 0; r < 7; r++)
            {
                for (int c = 0; c < 7; c++)
                {
                    convertedBoard[r, c] = (double)board[r, c];
                }
            }

            return convertedBoard;
        }
    }
}
