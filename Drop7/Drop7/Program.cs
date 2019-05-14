using Drop7.CNN;
using System;

namespace Drop7
{
    public class Program
    {
        private static bool useNN = true;

        static void Main(string[] args)
        {
            string restart = "";
            do 
            {
                Console.WriteLine();

                GameManager manager = new GameManager();
                ConvolutionalNeuralNetworkHelper networkHelper = new ConvolutionalNeuralNetworkHelper();

                manager.StartGame();

                manager.PrintNextTileText();

                while (manager.InProgress)
                {
                    Console.WriteLine("-------------------------------------------------------------------");
                    Console.WriteLine($"Score: {manager.MyGame.Score}");
                    Console.WriteLine("-------------------------------------------------------------------");

                    Console.Write("Enter Column (1 - 7) to drop number, non-# to quit game: ");
                    try
                    {
                        if (useNN)
                        {
                            try
                            {
                                int choice =
                                    networkHelper.GetNextResult(
                                        manager.MyGame.MyBoard.Tiles,
                                        manager.MyGame.MyBoard.NextTile);

                                Console.Write($"Neural Network chose {choice}");
                                manager.ProgressGame(choice);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.StackTrace);
                                throw e;
                            }
                        }
                        else
                        {
                            string userInput = Console.ReadLine();
                            int column = int.Parse(userInput);

                            if (column >= 1 && column <= 7 && manager.CheckValidInput(column))
                                manager.ProgressGame(column);
                            else
                                throw new Exception(userInput);
                        }
                    }
                    catch (Exception e)
                    {
                        int.TryParse(e.Message, out int val);

                        // if non-number
                        if (val == 0)
                        {
                            manager.EndGame();
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Enter a valid column");
                        }
                    }
                }

                Console.Write($"Enter 'r' to restart, or any other key to end{Environment.NewLine}");
                //restart = Console.ReadLine().ToString();
                restart = "r";
            } while (restart == "r");
        }
    }
}
