using System;
using System.Collections.Generic;
using System.Text;

namespace Drop7
{
    public class Game
    {
        public Game()
        {
            MyBoard = new Board();
        }

        public decimal Score { get; set; }
        public Board MyBoard { get; }

        public void Start()
        {
            Score = 0;
        }

        public void End()
        {

        }

        public bool CanDrop(int column)
        {
            return !MyBoard.IsColumnFull(column - 1);
        }

        public bool Progress(int column)
        {
            Score += MyBoard.AddTile(column);

            //CheckTurns
            //if 25th, 24th, 23rd...
            //  add new row to board
            //  get score from adding row

            return MyBoard.ValidBoard();
        }
    }
}
