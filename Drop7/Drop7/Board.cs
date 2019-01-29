using System;
using System.Collections.Generic;
using System.Text;

namespace Drop7
{
    public enum Tiles
    {
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Unbroken = 8,
        Cracked = 9,
        Empty = 10
    }

    public class Board
    {
        public Board()
        {
            _rand = new Random();
            _bustedOnUpdate = false;
            InitializeTiles();
            RandomizeTiles();
        }

        private Random _rand { get; }

        private bool _bustedOnUpdate { get; set; }

        private int _nextTile;
        public int NextTile
        {
            get
            {
                return _nextTile;
            }
            set
            {
                _nextTile = GetRandomTile();
            }
        }

        // numbered 1-7 with 0 == cracked, -1 == unbroken, -2 == 
        protected Tiles[,] Tiles { get; set; }

        private void InitializeTiles()
        {
            Tiles = new Tiles[7, 7];

            // blank board
            for (int r = 0; r < 7; r++)
                for (int c = 0; c < 7; c++)
                    Tiles[r, c] = Drop7.Tiles.Empty;
        }

        private int GetRandomTile()
        {
            return _rand.Next(7) + 1;
        }

        private void RandomizeTiles()
        {
            int[] tilesPerCol = { 4, 3, 3, 3, 2, 2, 2, 2, 2, 1, 1, 1, 1, 1, 0, 0, 0, 0 };

            for (int c = 0; c < 7; c++)
            {
                int height = tilesPerCol[_rand.Next(17) + 1];
                for (int r = 7 - height; r < 7; r++)
                {
                    // cast to enum
                    Tiles[r, c] = (Drop7.Tiles)(_rand.Next(8) + 1);
                }
            }
        }

        public void PrintBoard()
        {
            for (int r = 0; r < 7; r++)
            {
                Console.Write("{0,6}", 7 - r);
                for (int c = 0; c < 7; c++)
                {
                    if (Tiles[r, c] != Drop7.Tiles.Empty)
                        Console.Write("{0,5}", (int)Tiles[r, c]);
                    else
                        Console.Write("{0,5}", " ");
                }
                Console.Write($"{Environment.NewLine}{Environment.NewLine}");
            }
            Console.Write("{0,6}", "");
            for (int i = 1; i < 8; i++)
                Console.Write("{0,5}", i);
            Console.WriteLine();
        }

        public decimal AddTile(int column)
        {
            //push new tile on
            Stack<Tiles> tiles = new Stack<Tiles>();
            tiles.Push((Tiles)NextTile);
            // get existing tiles
            for (int i = 0; i < 7; i++)
            {
                Tiles tile = Tiles[i, column - 1];
                if (tile != Drop7.Tiles.Empty)
                    tiles.Push(tile);
            }
            // put updated stack back onto board
            for (int i = 6; i >= 0; i--)
            {
                if (tiles.Count > 0)
                    Tiles[i, column - 1] = tiles.Pop();
            }

            // trigger next tile update
            NextTile = 0;

            return UpdateBoard();
        }

        public decimal AddNewRow()
        {
            // add unbroken row to the bottom of tiles
            for (int i = 0; i < 7; i++)
            {
                RaiseColumnByOne(i);
            }

            return UpdateBoard();
        }

        // this is used by add new row
        private void RaiseColumnByOne(int column) // 0 - 6
        {
            // raise every value by one
            // check if it would bust, if bust, no processing
            if (IsColumnFull(column))
            {
                _bustedOnUpdate = true;
                return;
            }

            Stack<Tiles> tiles = new Stack<Tiles>();
            for (int i = 0; i < 7; i++)
            {
                Tiles tile = Tiles[i, column];
                if (tile != Drop7.Tiles.Empty)
                    tiles.Push(tile);
            }

            tiles.Push(Drop7.Tiles.Unbroken);

            for (int i = 6; i >= 0; i--)
            {
                if (tiles.Count > 0)
                    Tiles[i, column] = tiles.Pop();
            }
        }

        public bool IsColumnFull(int column) // 0 - 6
        {
            return Tiles[0,column] != Drop7.Tiles.Empty;
        }

        private bool IsBoardFull()
        {
            return IsColumnFull(0)
                && IsColumnFull(1)
                && IsColumnFull(2)
                && IsColumnFull(3)
                && IsColumnFull(4)
                && IsColumnFull(5)
                && IsColumnFull(6);
        }

        // has the board busted?
        public bool ValidBoard()
        {
            return _bustedOnUpdate || IsBoardFull();
        }

        private decimal UpdateBoard()
        {
            decimal score = 0;
            // resolve breakable tiles
            // resolve number tiles

            return score;
        }
    }
}
