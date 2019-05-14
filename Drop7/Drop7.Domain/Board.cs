using System;
using System.Collections.Generic;

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
            NextTile = 0; // trigger randomization on start
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

        // needed to expose this for NN metrics
        public Tiles[,] Tiles { get; set; }

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

            UpdateBoard();
        }

        public void PrintBoard()
        {
            var strings = new List<string>();
            PrintBoard(strings);
        }

        public void PrintBoard(List<string> strings)
        {
            string temp = "";
            for (int r = 0; r < 7; r++)
            {
                string row = "";
                temp = string.Format("{0,6}", 7 - r);
                row += temp;
                Console.Write(temp);
                for (int c = 0; c < 7; c++)
                {
                    if (Tiles[r, c] != Drop7.Tiles.Empty)
                    {
                        temp = string.Format("{0,5}", (int)Tiles[r, c]);
                        Console.Write(temp);
                        row += temp;
                    }
                    else
                    {
                        temp = string.Format("{0,5}", " ");
                        Console.Write(temp);
                        row += temp;
                    }
                }
                temp = $"{Environment.NewLine}{Environment.NewLine}";
                Console.Write(temp);
                strings.Add(row);
                strings.Add(temp);
            }
            temp = string.Format("{0,6}", "");
            Console.Write(temp);
            strings.Add(temp);

            string bottom = "";
            for (int i = 1; i < 8; i++)
            {
                temp = string.Format("{0,5}", i);
                Console.Write(temp);
                bottom += temp;
            }
            temp = $"{Environment.NewLine}";
            Console.Write(temp);
            strings.Add(bottom);
            strings.Add(temp);
        }

        // 1 through 7
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

            if (InvalidBoard)
                return 0;
            return UpdateBoard();
        }

        // this is used by add new row
        private void RaiseColumnByOne(int column) // 0 - 6
        {
            // raise every value in stack by one
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
        public bool InvalidBoard => _bustedOnUpdate || IsBoardFull();

        // is it completely empty?
        public bool IsBoardEmpty()
        {
            for (int r = 6; r >= 0; r--)
            {
                for (int c = 6; c >= 0; c--)
                {
                    // on first false we can return
                    if (Tiles[r, c] != Drop7.Tiles.Empty)
                        return false;
                }
            }

            return true;
        }

        public Dictionary<Tuple<int, int>, Tiles> GetOccupiedSlots()
        {
            Dictionary<Tuple<int, int>, Tiles> allTiles = new Dictionary<Tuple<int, int>, Tiles>();

            for (int r = 0; r < 7; r++)
            {
                for (int c = 0; c < 7; c++)
                {
                    if (Tiles[r, c] != Drop7.Tiles.Empty)
                        allTiles.Add(new Tuple<int, int>(r, c), Tiles[r, c]);
                }
            }

            return allTiles;
        }

        private decimal UpdateBoard(decimal? lastScore = null, int currentChain = 1)
        {
            if (lastScore != null && lastScore == 0)
            {
                if (IsBoardEmpty())
                    return 70000;
                return 0;
            }

            decimal score = 0;

            // get all tiles
            var tileInfo = GetOccupiedSlots();

            // keys to remove
            List<Tuple<int, int>> toRemove = new List<Tuple<int, int>>();

            // resolve number tiles
            foreach (var info in tileInfo)
            {
                if (info.Value == Drop7.Tiles.Unbroken || info.Value == Drop7.Tiles.Cracked)
                    continue;

                if (CanPop(info.Key.Item1, info.Key.Item2))
                    toRemove.Add(info.Key);
            }

            // remove all scored tiles and break any breakables nearby as we resolve
            foreach (var removeable in toRemove)
            {
                // check surrounding 4 for cracked or unbroken
                var tilesToBreak = CheckCrackedOrUnbroken(removeable.Item1, removeable.Item2);
                foreach (var tileToBreak in tilesToBreak)
                {
                    if (Tiles[tileToBreak.Item1, tileToBreak.Item2] == Drop7.Tiles.Unbroken)
                        Tiles[tileToBreak.Item1, tileToBreak.Item2] = Drop7.Tiles.Cracked;
                    else
                        Tiles[tileToBreak.Item1, tileToBreak.Item2] = (Drop7.Tiles)GetRandomTile();
                }

                // replace with empty tile
                Tiles[removeable.Item1, removeable.Item2] = Drop7.Tiles.Empty;
            }

            // condense board back down for next phase of gameplay (or more point scoring)
            CondenseTiles();

            // chain chain you know your name
            score += toRemove.Count * Chain(currentChain);

            return score + UpdateBoard(score, currentChain++);
        }

        private List<Tuple<int, int>> CheckCrackedOrUnbroken(int row, int col)
        {
            List<Tuple<int, int>> tilesToTransition = new List<Tuple<int, int>>();

            if (row != 0)
                if (Tiles[row - 1, col] == Drop7.Tiles.Cracked || Tiles[row - 1, col] == Drop7.Tiles.Unbroken)
                    tilesToTransition.Add(new Tuple<int, int>(row - 1, col));

            if (row != 6)
                if (Tiles[row + 1, col] == Drop7.Tiles.Cracked || Tiles[row + 1, col] == Drop7.Tiles.Unbroken)
                    tilesToTransition.Add(new Tuple<int, int>(row + 1, col));

            if (col != 0)
                if (Tiles[row, col - 1] == Drop7.Tiles.Cracked || Tiles[row, col - 1] == Drop7.Tiles.Unbroken)
                    tilesToTransition.Add(new Tuple<int, int>(row, col - 1));

            if (col != 6)
                if (Tiles[row, col + 1] == Drop7.Tiles.Cracked || Tiles[row, col + 1] == Drop7.Tiles.Unbroken)
                    tilesToTransition.Add(new Tuple<int, int>(row, col + 1));

            return tilesToTransition;
        }

        private bool CanPop(int row, int col)
        {
            return CheckColumnPop(row, col) || CheckRowPop(row, col);
        }

        private bool CheckColumnPop(int row, int col)
        {
            for (int r = 6; r >= 0; r--)
            {
                // we can exhaust the column, save a couple searches
                if (Tiles[r, col] == Drop7.Tiles.Empty)
                {
                    // we will always be one ahead
                    // this is always at max == 7 because we won't ever check blank columns
                    if (6 - r == (int)Tiles[row, col])
                        return true;
                    if (r != 0)
                        break;
                }
                else if (r == 0 && 7 == (int)Tiles[row, col])
                    return true;
            }
            return false;
        }

        private bool CheckRowPop(int row, int col)
        {
            bool maxWidthFound = false;
            int leftMost = col;
            int rightMost = col;
            while (!maxWidthFound)
            {
                int startingLeft = leftMost;
                int startingRight = rightMost;

                if (leftMost != 0)
                {
                    if (Tiles[row, leftMost - 1] != Drop7.Tiles.Empty)
                        leftMost--;
                }

                if (rightMost != 6)
                {
                    if (Tiles[row, rightMost + 1] != Drop7.Tiles.Empty)
                        rightMost++;
                }

                if (startingLeft == leftMost && startingRight == rightMost)
                    maxWidthFound = true;
            }

            return (rightMost - leftMost + 1) == (int)Tiles[row, col];
        }

        // coilumn by columne, we wipe out all the empty, and fill back in to be 7 tiles total, nothing fancy
        private void CondenseTiles()
        {
            CondenseColumn(0);
            CondenseColumn(1);
            CondenseColumn(2);
            CondenseColumn(3);
            CondenseColumn(4);
            CondenseColumn(5);
            CondenseColumn(6);
        }

        private void CondenseColumn(int column)
        {
            Stack<Tiles> tiles = new Stack<Tiles>();
            // get existing tiles
            for (int i = 0; i < 7; i++)
            {
                Tiles tile = Tiles[i, column];
                if (tile != Drop7.Tiles.Empty)
                    tiles.Push(tile);
            }
            // put updated stack back onto board
            for (int i = 6; i >= 0; i--)
            {
                if (tiles.Count > 0)
                    Tiles[i, column] = tiles.Pop();
                else
                    Tiles[i, column] = Drop7.Tiles.Empty;
            }
        }

        private int Chain(int length)
        {
            return 7 * (int)Math.Sqrt(length^5);
        }
    }
}
