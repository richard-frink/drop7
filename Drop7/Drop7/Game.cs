namespace Drop7
{
    public class Game
    {
        public Game()
        {
            MyBoard = new Board();
        }

        public int CurrentTurn { get; set; }
        private int MaxTurn { get; set; }
        private const int MinTurn = 5;
        public decimal Score { get; set; }
        public Board MyBoard { get; }

        public void Start()
        {
            Score = 0;
            MaxTurn = 25;
            CurrentTurn = 25;
        }

        public void End()
        {
            // nothing to clean up, could probably re-assign some roles to make this actually do something
            // but whatever
            // this is for fun
            // kind of
        }

        public bool CanDrop(int column)
        {
            return !MyBoard.IsColumnFull(column - 1);
        }

        public bool Progress(int column)
        {
            Score += MyBoard.AddTile(column);

            if (CurrentTurn == 0)
            {
                if (MaxTurn > MinTurn)
                {
                    MaxTurn--;
                }
                CurrentTurn = MaxTurn;
                Score += 7000;
                Score += MyBoard.AddNewRow();
            }

            return MyBoard.InvalidBoard;
        }
    }
}
