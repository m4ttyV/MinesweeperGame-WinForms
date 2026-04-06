namespace Saper.Models
{
    public class MinesweeperGame
    {
        private readonly Random _random = new();
        private bool _firstClick = true;

        public Cell[,] Grid { get; private set; }
        public GameSettings Settings { get; private set; }
        public int RemainingMines { get; private set; }
        public int RevealedCells { get; private set; }
        public GameState State { get; private set; }

        public event Action<Cell>? CellRevealed;
        public event Action<Cell>? CellFlagged;
        public event Action? GameWon;
        public event Action? GameLost;

        public MinesweeperGame(GameSettings settings)
        {
            Settings = settings;
            RemainingMines = settings.Mines;
            State = GameState.Playing;
            Grid = new Cell[settings.Rows, settings.Columns];
            InitializeGrid();
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
            if (State != GameState.Playing || !IsInside(row, col))
            {
                return;
            }

            var cell = Grid[row, col];
            if (cell.IsRevealed || cell.IsFlagged)
            {
                return;
            }

            if (_firstClick)
            {
                PlaceMines(row, col);
                _firstClick = false;
            }

            if (cell.IsMine)
            {
                cell.IsRevealed = true;
                State = GameState.Lost;
                RevealAllMines();
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

        public void RevealAdjacentCellsIfFlagsMatch(int row, int col)
        {
            if (State != GameState.Playing || !IsInside(row, col))
            {
                return;
            }

            var cell = Grid[row, col];
            if (!cell.IsRevealed || cell.AdjacentMines <= 0)
            {
                return;
            }

            int flaggedCount = GetAdjacentFlagCount(row, col);
            if (flaggedCount != cell.AdjacentMines)
            {
                return;
            }

            foreach (var (adjacentRow, adjacentCol) in GetAdjacentCells(row, col))
            {
                var adjacentCell = Grid[adjacentRow, adjacentCol];
                if (adjacentCell.IsRevealed || adjacentCell.IsFlagged)
                {
                    continue;
                }

                RevealCell(adjacentRow, adjacentCol);

                if (State != GameState.Playing)
                {
                    return;
                }
            }
        }

        public void ToggleFlag(int row, int col)
        {
            if (State != GameState.Playing || !IsInside(row, col))
            {
                return;
            }

            var cell = Grid[row, col];
            if (cell.IsRevealed)
            {
                return;
            }

            cell.IsFlagged = !cell.IsFlagged;
            RemainingMines += cell.IsFlagged ? -1 : 1;
            CellFlagged?.Invoke(cell);
        }

        public List<(int Row, int Col)> GetAdjacentCells(int row, int col)
        {
            var cells = new List<(int Row, int Col)>();

            for (int r = -1; r <= 1; r++)
            {
                for (int c = -1; c <= 1; c++)
                {
                    if (r == 0 && c == 0)
                    {
                        continue;
                    }

                    int newRow = row + r;
                    int newCol = col + c;

                    if (IsInside(newRow, newCol))
                    {
                        cells.Add((newRow, newCol));
                    }
                }
            }

            return cells;
        }

        public int GetAdjacentFlagCount(int row, int col)
        {
            if (!IsInside(row, col))
            {
                return 0;
            }

            int count = 0;
            foreach (var (adjacentRow, adjacentCol) in GetAdjacentCells(row, col))
            {
                if (Grid[adjacentRow, adjacentCol].IsFlagged)
                {
                    count++;
                }
            }

            return count;
        }

        private void RevealCellRecursive(int row, int col)
        {
            if (!IsInside(row, col))
            {
                return;
            }

            var cell = Grid[row, col];
            if (cell.IsRevealed || cell.IsFlagged)
            {
                return;
            }

            cell.IsRevealed = true;
            RevealedCells++;
            CellRevealed?.Invoke(cell);

            if (cell.AdjacentMines == 0)
            {
                foreach (var (adjacentRow, adjacentCol) in GetAdjacentCells(row, col))
                {
                    RevealCellRecursive(adjacentRow, adjacentCol);
                }
            }
        }

        private void PlaceMines(int safeRow, int safeCol)
        {
            var safeCells = GetAdjacentCells(safeRow, safeCol);
            safeCells.Add((safeRow, safeCol));

            var availableCells = new List<(int Row, int Col)>();

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

            var mines = availableCells.OrderBy(_ => _random.Next()).Take(Settings.Mines);

            foreach (var (row, col) in mines)
            {
                Grid[row, col].IsMine = true;
                UpdateAdjacentCounters(row, col);
            }
        }

        private void UpdateAdjacentCounters(int mineRow, int mineCol)
        {
            foreach (var (row, col) in GetAdjacentCells(mineRow, mineCol))
            {
                Grid[row, col].AdjacentMines++;
            }
        }

        private void RevealAllMines()
        {
            for (int row = 0; row < Settings.Rows; row++)
            {
                for (int col = 0; col < Settings.Columns; col++)
                {
                    if (Grid[row, col].IsMine)
                    {
                        Grid[row, col].IsRevealed = true;
                    }
                }
            }
        }

        private bool CheckWinCondition()
        {
            return RevealedCells == (Settings.Rows * Settings.Columns - Settings.Mines);
        }

        private bool IsInside(int row, int col)
        {
            return row >= 0 && row < Settings.Rows && col >= 0 && col < Settings.Columns;
        }
    }

    public enum GameState
    {
        Playing,
        Won,
        Lost
    }
}
