using System;

namespace Drop7
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine();

            GameManager manager = new GameManager();

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
                    string userInput = Console.ReadLine();
                    int column = int.Parse(userInput);

                    if (column >= 1 && column <= 7 && manager.CheckValidInput(column))
                        manager.ProgressGame(column);
                    else
                        throw new Exception(userInput);
                }
                catch (Exception e)
                {
                    int val;
                    int.TryParse(e.Message, out val);

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

            Console.Write("Press any key to end");
            Console.Read();
        }
    }
}
