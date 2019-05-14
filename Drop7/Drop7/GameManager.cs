using System;
using System.Collections.Generic;
using System.IO;

namespace Drop7
{
    public class GameManager
    {
        private Game _game;
        public Game MyGame
        {
            get
            {
                if (_game != null)
                    return _game;
                return new Game();
            }
        }

        public bool InProgress { get; set; }

        // this could later abstract out to variuous game types, but this will be merely a pass through for now
        public GameManager()
        {
            _game = new Game();
        }

        public void StartGame()
        {
            _game.Start();
            _game.MyBoard.PrintBoard();
            InProgress = true;
        }

        public bool CheckValidInput(int column)
        {
            return MyGame.CanDrop(column);
        }

        public void PrintNextTileText()
        {
            Console.WriteLine();
            Console.WriteLine("-------------------------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine($"The next tile to be placed is {MyGame.MyBoard.NextTile}");
            Console.WriteLine();
            Console.WriteLine($"Your current turn is {MyGame.CurrentTurn}");
            Console.WriteLine();
        }

        public void ProgressGame(int column)
        {
            if (MyGame.Progress(column))
                EndGame();
            else
            {
                PrintNextTileText();
                MyGame.MyBoard.PrintBoard();
            }
        }

        public void EndGame()
        {
            InProgress = false;
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Game over");
            Console.WriteLine();
            Console.WriteLine($"Final Score: {MyGame.Score}");

            WriteToLog();
        }

        public void WriteToLog()
        {
            string path = @"D:\Drop7-Results\";
            path += $"results-{DateTime.Now.ToString("yyyy-MMM-dd-HH")}.txt";

            List<string> strings = new List<string>();
            strings.Add(string.Format($"{Environment.NewLine}Begin Results"));
            MyGame.MyBoard.PrintBoard(strings);
            strings.Add(string.Format($"Final Score: {MyGame.Score}"));
            strings.Add(string.Format($"End Results{Environment.NewLine}"));

            File.AppendAllLines(path, strings);
        }
    }
}
