namespace Saper.Models
{
    public class MinesweeperGame
    {
        public Cell[,] Grid { get; private set; }
        public GameSettings Settings { get; private set; }
        public GameState State { get; private set; }
        public int RemainingMines { get; private set; }
        public int RevealedCells { get; private set; }

        private bool _firstClick = true;
        private Random _random = new Random();

        public event Action<Cell> CellRevealed;
        public event Action<Cell> CellFlagged;
        public event Action GameWon;
        public event Action GameLost;

        public MinesweeperGame(GameSettings settings)
        {
            Settings = settings;
            InitializeGrid();
            State = GameState.Playing;
            RemainingMines = Settings.Mines;
        }

        private void InitializeGrid()
        {
            Grid = new Cell[Settings.Rows, Settings.Columns];

            for (int row = 0; row < Settings.Rows; row++)
            {
                for (int col = 0; col < Settings.Columns; col++)
                {
                    Grid[row, col] = new Cell(row, col);
                }
            }
        }

        public void RevealCell(int row, int col)
        {
            if (State != GameState.Playing || row < 0 || row >= Settings.Rows || col < 0 || col >= Settings.Columns)
                return;

            var cell = Grid[row, col];
            if (cell.IsRevealed || cell.IsFlagged)
                return;

            if (_firstClick)
            {
                PlaceMines(row, col);
                _firstClick = false;
            }

            if (cell.IsMine)
            {
                State = GameState.Lost;
                GameLost?.Invoke();
                return;
            }

            RevealCellRecursive(row, col);

            if (CheckWinCondition())
            {
                State = GameState.Won;
                GameWon?.Invoke();
            }
        }

        private void RevealCellRecursive(int row, int col)
        {
            if (row < 0 || row >= Settings.Rows || col < 0 || col >= Settings.Columns)
                return;

            var cell = Grid[row, col];
            if (cell.IsRevealed || cell.IsFlagged)
                return;

            cell.IsRevealed = true;
            RevealedCells++;
            CellRevealed?.Invoke(cell);

            if (cell.AdjacentMines == 0)
            {
                for (int r = -1; r <= 1; r++)
                {
                    for (int c = -1; c <= 1; c++)
                    {
                        if (r == 0 && c == 0) continue;
                        RevealCellRecursive(row + r, col + c);
                    }
                }
            }
        }

        public void ToggleFlag(int row, int col)
        {
            if (State != GameState.Playing || row < 0 || row >= Settings.Rows || col < 0 || col >= Settings.Columns)
                return;

            var cell = Grid[row, col];
            if (cell.IsRevealed)
                return;

            cell.IsFlagged = !cell.IsFlagged;
            RemainingMines += cell.IsFlagged ? -1 : 1;
            CellFlagged?.Invoke(cell);
        }

        private void PlaceMines(int safeRow, int safeCol)
        {
            var safeCells = GetAdjacentCells(safeRow, safeCol);
            safeCells.Add((safeRow, safeCol));

            var availableCells = new List<(int, int)>();

            for (int row = 0; row < Settings.Rows; row++)
            {
                for (int col = 0; col < Settings.Columns; col++)
                {
                    if (!safeCells.Contains((row, col)))
                    {
                        availableCells.Add((row, col));
                    }
                }
            }

            // Перемешиваем и выбираем нужное количество мин
            var mines = availableCells.OrderBy(x => _random.Next()).Take(Settings.Mines);

            foreach (var (row, col) in mines)
            {
                Grid[row, col].IsMine = true;

                // Обновляем счётчик мин у соседних клеток
                UpdateAdjacentCounters(row, col);
            }
        }

        private List<(int, int)> GetAdjacentCells(int row, int col)
        {
            var cells = new List<(int, int)>();

            for (int r = -1; r <= 1; r++)
            {
                for (int c = -1; c <= 1; c++)
                {
                    int newRow = row + r;
                    int newCol = col + c;

                    if (newRow >= 0 && newRow < Settings.Rows && newCol >= 0 && newCol < Settings.Columns)
                    {
                        cells.Add((newRow, newCol));
                    }
                }
            }

            return cells;
        }

        private void UpdateAdjacentCounters(int mineRow, int mineCol)
        {
            for (int r = -1; r <= 1; r++)
            {
                for (int c = -1; c <= 1; c++)
                {
                    int row = mineRow + r;
                    int col = mineCol + c;

                    if (row >= 0 && row < Settings.Rows && col >= 0 && col < Settings.Columns)
                    {
                        Grid[row, col].AdjacentMines++;
                    }
                }
            }
        }

        private bool CheckWinCondition()
        {
            return RevealedCells == (Settings.Rows * Settings.Columns - Settings.Mines);
        }
    }
    public enum GameState
    {
        Playing,
        Won,
        Lost
    }
}
