using Saper.Models;
using Saper.ViewModels;
using System.Drawing.Drawing2D;
using System.Reflection;

namespace Saper
{
    public partial class Form1 : Form
    {
        private const int BoardPadding = 18;
        private const int OuterPadding = 12;
        private const int MinimumWindowWidth = 520;
        private const int HeaderHeight = 160;

        private readonly MainViewModel _viewModel;
        private DataGridView? _gridView;
        private int _currentCellSize;
        private bool _suppressDifficultyChange;
        private bool _isRefreshingBoard;
        private readonly HashSet<(int Row, int Col)> _highlightedCells = new();
        private bool _isHoldingNumberCell;

        public bool eng = true;
        public Dictionary<int, string> rusCongratulations = new();
        public Dictionary<int, string> rusCondolences = new();
        public Dictionary<int, string> engCongratulations = new();
        public Dictionary<int, string> engCondolences = new();

        public Form1()
        {
            LoadLocalizedMessages();

            _viewModel = new MainViewModel(rusCondolences, rusCongratulations, engCondolences, engCongratulations, eng);

            InitializeComponent();
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            ApplyTheme();
            ApplyLocalization();
            RefreshBoardLayout();

            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void LoadLocalizedMessages()
        {
            engCondolences = LoadMessages("En", "Condolences.xam");
            engCongratulations = LoadMessages("En", "Congratulations.xam");
            rusCondolences = LoadMessages("Ru", "Condolences.xam");
            rusCongratulations = LoadMessages("Ru", "Congratulations.xam");
        }

        private static Dictionary<int, string> LoadMessages(string languageFolder, string fileName)
        {
            var result = new Dictionary<int, string>();
            var path = Path.Combine(AppContext.BaseDirectory, "Recources", languageFolder, fileName);

            if (!File.Exists(path))
            {
                return result;
            }

            using var reader = new StreamReader(path);
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                result[result.Count] = line.Trim();
            }

            return result;
        }

        private void ApplyTheme()
        {
            BackColor = Color.FromArgb(246, 248, 252);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            MinimumSize = new Size(MinimumWindowWidth, 460);

            _controlPanel.BackColor = Color.White;
            _controlPanel.BorderStyle = BorderStyle.FixedSingle;

            _boardPanel.BackColor = Color.White;
            _boardPanel.BorderStyle = BorderStyle.FixedSingle;

            _titleLabel.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold, GraphicsUnit.Point);
            _titleLabel.ForeColor = Color.FromArgb(29, 37, 53);

            _hintLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            _hintLabel.ForeColor = Color.FromArgb(107, 114, 128);

            _minesLabel.TextAlign = ContentAlignment.MiddleCenter;
            _minesLabel.Font = new Font("Segoe UI Semibold", 10.5F, FontStyle.Bold, GraphicsUnit.Point);
            _minesLabel.ForeColor = Color.FromArgb(52, 72, 120);
            _minesLabel.BackColor = Color.FromArgb(233, 239, 249);
            _minesLabel.BorderStyle = BorderStyle.FixedSingle;

            ConfigureButton(_newGameButton, Color.FromArgb(99, 122, 173), Color.White);
            ConfigureButton(_langChangerButton, Color.FromArgb(238, 242, 248), Color.FromArgb(65, 76, 99));

            _difficultyComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            _difficultyComboBox.FlatStyle = FlatStyle.Flat;
            _difficultyComboBox.BackColor = Color.White;
            _difficultyComboBox.ForeColor = Color.FromArgb(31, 41, 55);
            _difficultyComboBox.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);

            ApplyRoundedCorners(_controlPanel, 16);
            ApplyRoundedCorners(_boardPanel, 16);
            ApplyRoundedCorners(_newGameButton, 12);
            ApplyRoundedCorners(_langChangerButton, 12);
            ApplyRoundedCorners(_minesLabel, 12);
        }

        private static void ConfigureButton(Button button, Color background, Color foreground)
        {
            button.BackColor = background;
            button.ForeColor = foreground;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Cursor = Cursors.Hand;
            button.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold, GraphicsUnit.Point);
            button.TabStop = false;
        }

        private static void ApplyRoundedCorners(Control control, int radius)
        {
            if (control.Width <= 0 || control.Height <= 0)
            {
                return;
            }

            using var path = new GraphicsPath();
            int diameter = radius * 2;
            var rect = new Rectangle(0, 0, control.Width, control.Height);

            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            control.Region = new Region(path);
        }

        private void SetupDataGridView()
        {
            if (_gridView != null)
            {
                _boardPanel.Controls.Remove(_gridView);
                _gridView.CellClick -= GridView_CellClick;
                _gridView.CellMouseClick -= GridView_CellMouseClick;
                _gridView.CellMouseDown -= GridView_CellMouseDown;
                _gridView.CellMouseUp -= GridView_CellMouseUp;
                _gridView.MouseLeave -= GridView_MouseLeave;
                _gridView.Dispose();
            }

            _gridView = new DataGridView
            {
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                AllowUserToResizeColumns = false,
                AllowUserToOrderColumns = false,
                RowHeadersVisible = false,
                ColumnHeadersVisible = false,
                ReadOnly = true,
                MultiSelect = false,
                ScrollBars = ScrollBars.None,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.Single,
                SelectionMode = DataGridViewSelectionMode.CellSelect,
                BackgroundColor = Color.White,
                GridColor = Color.FromArgb(208, 216, 227),
                TabStop = false,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    SelectionBackColor = Color.White,
                    SelectionForeColor = Color.FromArgb(29, 37, 53),
                    Font = new Font("Segoe UI Semibold", 10.5F, FontStyle.Bold, GraphicsUnit.Point)
                }
            };

            EnableDoubleBuffer(_gridView);
            ConfigureGridView();
            _boardPanel.Controls.Add(_gridView);
            _gridView.BringToFront();
        }

        private static void EnableDoubleBuffer(DataGridView gridView)
        {
            try
            {
                typeof(DataGridView)
                    .GetProperty("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance)
                    ?.SetValue(gridView, true, null);
            }
            catch
            {
            }
        }

        private void ConfigureGridView()
        {
            if (_gridView == null)
            {
                return;
            }

            _gridView.Columns.Clear();
            _gridView.Rows.Clear();
            _highlightedCells.Clear();
            _isHoldingNumberCell = false;

            var settings = _viewModel.CurrentSettings;
            _currentCellSize = GetCellSize(settings);

            for (int col = 0; col < settings.Columns; col++)
            {
                var column = new DataGridViewButtonColumn
                {
                    Width = _currentCellSize,
                    FlatStyle = FlatStyle.Flat,
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        Alignment = DataGridViewContentAlignment.MiddleCenter,
                        BackColor = Color.FromArgb(221, 229, 239),
                        ForeColor = Color.FromArgb(29, 37, 53),
                        SelectionBackColor = Color.FromArgb(221, 229, 239),
                        SelectionForeColor = Color.FromArgb(29, 37, 53)
                    }
                };

                _gridView.Columns.Add(column);
            }

            _gridView.Rows.Add(settings.Rows);

            foreach (DataGridViewRow row in _gridView.Rows)
            {
                row.Height = _currentCellSize;
            }

            _gridView.Width = settings.Columns * _currentCellSize;
            _gridView.Height = settings.Rows * _currentCellSize;

            _gridView.CellClick += GridView_CellClick;
            _gridView.CellMouseClick += GridView_CellMouseClick;
            _gridView.CellMouseDown += GridView_CellMouseDown;
            _gridView.CellMouseUp += GridView_CellMouseUp;
            _gridView.MouseLeave += GridView_MouseLeave;

            UpdateGridAppearance();
        }

        private static int GetCellSize(GameSettings settings)
        {
            if (settings.Columns >= 30)
            {
                return 28;
            }

            if (settings.Rows >= 16)
            {
                return 30;
            }

            return 34;
        }

        private void RefreshBoardLayout()
        {
            _isRefreshingBoard = true;
            try
            {
                SuspendLayout();
                SetupDataGridView();
                UpdateFormSize();
                UpdateGridAppearance();
            }
            finally
            {
                ResumeLayout(true);
                _isRefreshingBoard = false;
            }
        }

        private void UpdateGridAppearance()
        {
            if (_viewModel.Game == null || _gridView == null)
            {
                return;
            }

            if (_gridView.RowCount != _viewModel.Game.Settings.Rows || _gridView.ColumnCount != _viewModel.Game.Settings.Columns)
            {
                return;
            }

            for (int row = 0; row < _viewModel.Game.Settings.Rows; row++)
            {
                for (int col = 0; col < _viewModel.Game.Settings.Columns; col++)
                {
                    var cell = _viewModel.Game.Grid[row, col];
                    var gridCell = _gridView[col, row];

                    gridCell.Style.Padding = Padding.Empty;
                    gridCell.Style.Font = new Font("Segoe UI Semibold", 10.5F, FontStyle.Bold, GraphicsUnit.Point);

                    if (cell.IsMine && (_viewModel.Game.State == GameState.Lost || cell.IsRevealed))
                    {
                        gridCell.Style.BackColor = Color.FromArgb(252, 232, 232);
                        gridCell.Style.SelectionBackColor = Color.FromArgb(252, 232, 232);
                        gridCell.Style.ForeColor = Color.FromArgb(185, 28, 28);
                        gridCell.Style.SelectionForeColor = Color.FromArgb(185, 28, 28);
                        gridCell.Value = "💣";
                        continue;
                    }

                    if (cell.IsRevealed)
                    {
                        var revealedColor = cell.AdjacentMines == 0
                            ? Color.FromArgb(248, 250, 253)
                            : Color.FromArgb(253, 254, 255);

                        gridCell.Style.BackColor = revealedColor;
                        gridCell.Style.SelectionBackColor = revealedColor;
                        gridCell.Style.ForeColor = GetNumberColor(cell.AdjacentMines);
                        gridCell.Style.SelectionForeColor = GetNumberColor(cell.AdjacentMines);
                        gridCell.Value = cell.AdjacentMines > 0 ? cell.AdjacentMines.ToString() : string.Empty;
                    }
                    else if (cell.IsFlagged)
                    {
                        var flaggedColor = _highlightedCells.Contains((row, col))
                            ? Color.FromArgb(255, 236, 204)
                            : Color.FromArgb(255, 244, 229);

                        gridCell.Style.BackColor = flaggedColor;
                        gridCell.Style.SelectionBackColor = flaggedColor;
                        gridCell.Style.ForeColor = Color.FromArgb(201, 105, 25);
                        gridCell.Style.SelectionForeColor = Color.FromArgb(201, 105, 25);
                        gridCell.Value = "⚑";
                    }
                    else
                    {
                        var hiddenColor = _highlightedCells.Contains((row, col))
                            ? Color.FromArgb(210, 225, 245)
                            : Color.FromArgb(221, 229, 239);

                        gridCell.Style.BackColor = hiddenColor;
                        gridCell.Style.SelectionBackColor = hiddenColor;
                        gridCell.Style.ForeColor = Color.FromArgb(29, 37, 53);
                        gridCell.Style.SelectionForeColor = Color.FromArgb(29, 37, 53);
                        gridCell.Value = string.Empty;
                    }
                }
            }

            _gridView.ClearSelection();
            _gridView.CurrentCell = null;

            _minesLabel.Text = eng
                ? $"Mines left: {_viewModel.Game.RemainingMines}"
                : $"Осталось мин: {_viewModel.Game.RemainingMines}";
        }

        private static Color GetNumberColor(int number)
        {
            return number switch
            {
                1 => Color.FromArgb(37, 99, 235),
                2 => Color.FromArgb(22, 163, 74),
                3 => Color.FromArgb(220, 38, 38),
                4 => Color.FromArgb(79, 70, 229),
                5 => Color.FromArgb(190, 24, 93),
                6 => Color.FromArgb(8, 145, 178),
                7 => Color.FromArgb(55, 65, 81),
                8 => Color.FromArgb(100, 116, 139),
                _ => Color.FromArgb(29, 37, 53)
            };
        }

        private void GridView_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            // Left-click opening is handled in CellMouseClick so we can reliably detect the mouse button.
        }

        private void GridView_CellMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0 || _viewModel.Game == null)
            {
                return;
            }

            var cell = _viewModel.Game.Grid[e.RowIndex, e.ColumnIndex];

            if (e.Button == MouseButtons.Left)
            {
                if (!cell.IsRevealed)
                {
                    _viewModel.OnCellLeftClick(e.RowIndex, e.ColumnIndex);
                    UpdateGridAppearance();
                }

                return;
            }

            if (e.Button != MouseButtons.Right)
            {
                return;
            }

            if (cell.IsRevealed && cell.AdjacentMines > 0)
            {
                _viewModel.Game.RevealAdjacentCellsIfFlagsMatch(e.RowIndex, e.ColumnIndex);
            }
            else
            {
                _viewModel.OnCellRightClick(e.RowIndex, e.ColumnIndex);
            }

            UpdateGridAppearance();
        }

        private void GridView_CellMouseDown(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;
            }

            var cell = _viewModel.Game.Grid[e.RowIndex, e.ColumnIndex];
            if (!cell.IsRevealed || cell.AdjacentMines <= 0)
            {
                return;
            }

            _isHoldingNumberCell = true;
            SetHighlightedNeighbours(e.RowIndex, e.ColumnIndex);
        }

        private void GridView_CellMouseUp(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            ClearHighlightedNeighbours();
        }

        private void GridView_MouseLeave(object? sender, EventArgs e)
        {
            ClearHighlightedNeighbours();
        }

        private void SetHighlightedNeighbours(int row, int col)
        {
            _highlightedCells.Clear();

            foreach (var (adjacentRow, adjacentCol) in _viewModel.Game.GetAdjacentCells(row, col))
            {
                if (!_viewModel.Game.Grid[adjacentRow, adjacentCol].IsRevealed)
                {
                    _highlightedCells.Add((adjacentRow, adjacentCol));
                }
            }

            UpdateGridAppearance();
        }

        private void ClearHighlightedNeighbours()
        {
            if (!_isHoldingNumberCell && _highlightedCells.Count == 0)
            {
                return;
            }

            _isHoldingNumberCell = false;
            if (_highlightedCells.Count == 0)
            {
                return;
            }

            _highlightedCells.Clear();
            UpdateGridAppearance();
        }

        private void DifficultyComboBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_suppressDifficultyChange || _difficultyComboBox.SelectedIndex < 0)
            {
                return;
            }

            var settings = _difficultyComboBox.SelectedIndex switch
            {
                0 => _viewModel.BeginnerSettings,
                1 => _viewModel.IntermediateSettings,
                2 => _viewModel.ExpertSettings,
                _ => _viewModel.BeginnerSettings
            };

            _viewModel.OnChangeDifficulty(settings);
        }

        private void NewGameButton_Click(object? sender, EventArgs e)
        {
            _viewModel.OnNewGame();
        }

        private void UpdateFormSize()
        {
            if (_viewModel.CurrentSettings == null || _gridView == null)
            {
                return;
            }

            int gridWidth = _viewModel.CurrentSettings.Columns * _currentCellSize;
            int gridHeight = _viewModel.CurrentSettings.Rows * _currentCellSize;
            int boardWidth = gridWidth + BoardPadding * 2;
            int boardHeight = gridHeight + BoardPadding * 2;

            int contentWidth = Math.Max(MinimumWindowWidth, boardWidth + OuterPadding * 2 + 24);
            int contentHeight = HeaderHeight + boardHeight + OuterPadding * 2 + 14;

            ClientSize = new Size(contentWidth, contentHeight);

            _controlPanel.Location = new Point(OuterPadding, OuterPadding);
            _controlPanel.Size = new Size(ClientSize.Width - OuterPadding * 2, HeaderHeight - 12);

            UpdateHeaderLayout();

            _boardPanel.Size = new Size(boardWidth, boardHeight);
            _boardPanel.Location = new Point((ClientSize.Width - _boardPanel.Width) / 2, _controlPanel.Bottom + 14);

            _gridView.Location = new Point(BoardPadding, BoardPadding);

            ApplyRoundedCorners(_controlPanel, 16);
            ApplyRoundedCorners(_boardPanel, 16);
            ApplyRoundedCorners(_newGameButton, 12);
            ApplyRoundedCorners(_langChangerButton, 12);
            ApplyRoundedCorners(_minesLabel, 12);
        }

        private void UpdateHeaderLayout()
        {
            _titleLabel.Location = new Point(24, 18);
            _titleLabel.Size = new Size(_controlPanel.Width - 220, 34);

            _difficultyComboBox.Location = new Point(24, 65);
            _difficultyComboBox.Size = new Size(220, 36);

            _newGameButton.Location = new Point(_difficultyComboBox.Right + 12, 65);
            _newGameButton.Size = new Size(140, 36);

            _langChangerButton.Location = new Point(_newGameButton.Right + 12, 65);
            _langChangerButton.Size = new Size(76, 36);

            _minesLabel.Location = new Point(_controlPanel.Width - 176, 24);
            _minesLabel.Size = new Size(152, 34);

            _hintLabel.Location = new Point(24, 110);
            _hintLabel.Size = new Size(_controlPanel.Width - 48, 40);
        }

        private void ApplyLocalization()
        {
            Text = eng ? "Minesweeper" : "Сапёр";
            _titleLabel.Text = eng ? "Minesweeper" : "Сапёр";
            _hintLabel.Text = eng
                ? "LMB — open • RMB — flag / chord • Hold LMB on a number to preview neighbours"
                : "ЛКМ — открыть • ПКМ — флаг / автооткрытие • Удерживай ЛКМ на числе для подсветки";
            _newGameButton.Text = eng ? "New game" : "Новая игра";
            _langChangerButton.Text = eng ? "RU" : "EN";

            int selectedIndex = GetDifficultyIndex();
            _suppressDifficultyChange = true;
            _difficultyComboBox.Items.Clear();
            _difficultyComboBox.Items.AddRange(eng
                ? new object[]
                {
                    "Beginner (9×9 • 10 mines)",
                    "Amateur (16×16 • 40 mines)",
                    "Expert (16×30 • 99 mines)"
                }
                : new object[]
                {
                    "Новичок (9×9 • 10 мин)",
                    "Любитель (16×16 • 40 мин)",
                    "Эксперт (16×30 • 99 мин)"
                });
            _difficultyComboBox.SelectedIndex = selectedIndex;
            _suppressDifficultyChange = false;

            UpdateGridAppearance();
        }

        private int GetDifficultyIndex()
        {
            if (_viewModel.CurrentSettings.Rows == 16 && _viewModel.CurrentSettings.Columns == 16)
            {
                return 1;
            }

            if (_viewModel.CurrentSettings.Rows == 16 && _viewModel.CurrentSettings.Columns == 30)
            {
                return 2;
            }

            return 0;
        }

        private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(MainViewModel.Game))
            {
                return;
            }

            if (IsDisposed || _isRefreshingBoard)
            {
                return;
            }

            void RefreshView()
            {
                if (_isRefreshingBoard || _viewModel.Game == null)
                {
                    return;
                }

                _isHoldingNumberCell = false;
                _highlightedCells.Clear();

                bool dimensionsMismatch = _gridView == null
                    || _gridView.RowCount != _viewModel.Game.Settings.Rows
                    || _gridView.ColumnCount != _viewModel.Game.Settings.Columns;

                if (dimensionsMismatch)
                {
                    RefreshBoardLayout();
                    return;
                }

                UpdateGridAppearance();
                UpdateFormSize();
            }

            if (InvokeRequired)
            {
                BeginInvoke((System.Windows.Forms.MethodInvoker)(RefreshView));
                return;
            }

            RefreshView();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (_viewModel.CurrentSettings == null || _gridView == null || _isRefreshingBoard)
            {
                return;
            }

            UpdateFormSize();
        }

        private void Form1_Load(object? sender, EventArgs e)
        {
            ApplyTheme();
            ApplyLocalization();
            UpdateFormSize();
        }

        private void LangChanger_Click(object? sender, EventArgs e)
        {
            eng = !eng;
            _viewModel.SetLanguage(eng);
            ApplyLocalization();
        }
    }
}
