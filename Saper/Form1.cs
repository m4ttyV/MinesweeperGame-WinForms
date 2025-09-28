using Saper.ViewModels;

namespace Saper
{
    public partial class Form1 : Form
    {
        private MainViewModel _viewModel;
        private DataGridView _gridView;
        private ComboBox _difficultyComboBox;
        private Button _newGameButton;
        private Label _minesLabel;
        public bool eng = true;
        public Dictionary<int, string> rusCongratulations = new Dictionary<int, string>();
        public Dictionary<int, string> rusCondolences = new Dictionary<int, string>();
        public Dictionary<int, string> engCongratulations = new Dictionary<int, string>();
        public Dictionary<int, string> engCondolences = new Dictionary<int, string>();
        public Form1()
        {

            using (StreamReader reader = new StreamReader("./Recources/En/Condolences.xam"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    engCondolences[engCondolences.Count] = line;
                }
            }
            using (StreamReader reader = new StreamReader("./Recources/En/Congratulations.xam"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    engCongratulations[engCongratulations.Count] = line;
                }
            }
            using (StreamReader reader = new StreamReader("./Recources/Ru/Condolences.xam"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    rusCondolences[rusCondolences.Count] = line;
                }
            }
            using (StreamReader reader = new StreamReader("./Recources/Ru/Congratulations.xam"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    rusCongratulations[rusCongratulations.Count] = line;
                }
            }
            _viewModel = new MainViewModel(rusCondolences, rusCongratulations, engCondolences, engCongratulations, eng);
            InitializeComponent();
            SetupDataGridView();
            UpdateFormSize();

            // Подписываемся на изменения игры
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void SetupDataGridView()
        {
            if (_gridView != null)
            {
                this.Controls.Remove(_gridView);
                _gridView.Dispose();
            }

            _gridView = new DataGridView
            {
                Location = new Point(10, 50), // Отступ от верха с учетом панели управления
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                AllowUserToResizeColumns = false,
                RowHeadersVisible = false,
                ColumnHeadersVisible = false,
                ReadOnly = true,
                ScrollBars = ScrollBars.None,
                BackgroundColor = SystemColors.Control
            };

            ConfigureGridView();
            this.Controls.Add(_gridView);
            _gridView.BringToFront(); // Убедимся, что грид поверх других элементов
        }

        private void ConfigureGridView()
        {
            _gridView.Columns.Clear();
            _gridView.Rows.Clear();

            var settings = _viewModel.CurrentSettings;

            // Настраиваем столбцы
            for (int col = 0; col < settings.Columns; col++)
            {
                _gridView.Columns.Add(new DataGridViewButtonColumn
                {
                    Width = 30,
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        Alignment = DataGridViewContentAlignment.MiddleCenter,
                        BackColor = Color.LightGray,
                        SelectionBackColor = Color.LightGray
                    }
                });
            }

            // Добавляем строки
            _gridView.Rows.Add(settings.Rows);

            // Настраиваем высоту строк
            foreach (DataGridViewRow row in _gridView.Rows)
            {
                row.Height = 30;
            }

            // Устанавливаем правильный размер DataGridView
            _gridView.Width = settings.Columns * 30;
            _gridView.Height = settings.Rows * 30;

            // Подписываемся на события
            _gridView.CellClick += GridView_CellClick;
            _gridView.CellMouseClick += GridView_CellMouseClick;

            UpdateGridAppearance();
        }

        private void UpdateGridAppearance()
        {
            if (_viewModel.Game == null) return;
            while (_viewModel.Game.Settings.Columns > _gridView.Columns.Count)
            {

                DataGridViewColumn column = new DataGridViewColumn();
                var cellType = typeof(DataGridViewButtonCell);
                column.CellTemplate = (DataGridViewCell)Activator.CreateInstance(cellType);
                _gridView.Columns.Add(column);
            }
            while (_viewModel.Game.Settings.Rows > _gridView.Rows.Count)
            {

                _gridView.Rows.Add(1);
            }
            for (int row = 0; row < _viewModel.Game.Settings.Rows; row++)
            {
                for (int col = 0; col < _viewModel.Game.Settings.Columns; col++)
                {
                    var cell = _viewModel.Game.Grid[row, col];
                    var gridCell = _gridView[col, row];

                    if (cell.IsRevealed)
                    {
                        if (cell.IsMine)
                        {
                            gridCell.Style.BackColor = Color.Red;
                            gridCell.Value = "💣";
                        }
                        else
                        {
                            gridCell.Style.BackColor = Color.Green;
                            gridCell.Value = cell.AdjacentMines > 0 ? cell.AdjacentMines.ToString() : "";
                            gridCell.Style.ForeColor = GetNumberColor(cell.AdjacentMines);
                        }
                    }
                    else
                    {
                        gridCell.Style.BackColor = Color.LightBlue;
                        gridCell.Value = cell.IsFlagged ? "🚩" : "";
                    }
                }
            }
            if (eng)
                _minesLabel.Text = $"Mines: {_viewModel.Game.RemainingMines}";
            else
                _minesLabel.Text = $"Мин: {_viewModel.Game.RemainingMines}";
        }

        private Color GetNumberColor(int number)
        {
            return number switch
            {
                1 => Color.Blue,
                2 => Color.Green,
                3 => Color.Red,
                4 => Color.DarkBlue,
                5 => Color.DarkRed,
                6 => Color.Teal,
                7 => Color.Black,
                8 => Color.Gray,
                _ => Color.Black
            };
        }

        private void GridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                var coordinates = (e.RowIndex, e.ColumnIndex);
                _viewModel.OnCellLeftClick(e.RowIndex, e.ColumnIndex);
                UpdateGridAppearance();
            }
        }

        private void GridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                _viewModel.OnCellRightClick(e.RowIndex, e.ColumnIndex);
                UpdateGridAppearance();
            }
        }

        private void DifficultyComboBox_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            var settings = _difficultyComboBox.SelectedIndex switch
            {
                0 => _viewModel.BeginnerSettings,
                1 => _viewModel.IntermediateSettings,
                2 => _viewModel.ExpertSettings,
                _ => _viewModel.BeginnerSettings
            };

            _viewModel.OnChangeDifficulty(settings);
            SetupDataGridView();
            UpdateFormSize();
        }

        private void NewGameButton_Click(object sender, System.EventArgs e)
        {
            _viewModel.OnNewGame();
            SetupDataGridView();
            UpdateFormSize();
        }

        private void UpdateFormSize()
        {
            if (_viewModel.CurrentSettings == null) return;

            var settings = _viewModel.CurrentSettings;

            // Рассчитываем размер формы на основе размера поля
            int cellSize = 30; // размер одной клетки
            int margin = 20; // отступы
            int controlPanelHeight = 60; // высота панели управления

            int gridWidth = settings.Columns * cellSize;
            int gridHeight = settings.Rows * cellSize;

            // Устанавливаем размер формы
            this.ClientSize = new Size(
                gridWidth + margin * 2,
                gridHeight + controlPanelHeight + margin
            );

            // Центрируем DataGridView в форме
            if (_gridView != null)
            {
                _gridView.Location = new Point(
                    (this.ClientSize.Width - gridWidth) / 2,
                    controlPanelHeight
                );
            }
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainViewModel.Game) || e.PropertyName == nameof(MainViewModel.CurrentSettings))
            {
                // При изменении игры или настроек обновляем интерфейс
                this.Invoke((MethodInvoker)delegate
                {
                    UpdateGridAppearance();
                    UpdateFormSize();
                });
            }
        }

        protected override void OnResize(System.EventArgs e)
        {
            base.OnResize(e);
            // При изменении размера формы центрируем поле
            if (_viewModel?.CurrentSettings != null && _gridView != null)
            {
                int gridWidth = _viewModel.CurrentSettings.Columns * 30;
                int controlPanelHeight = 60;

                _gridView.Location = new Point(
                    (this.ClientSize.Width - gridWidth) / 2,
                    controlPanelHeight
                );
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _difficultyComboBox.Items.AddRange(new object[] { "Beginner (9x9, 10 min)", "Amateur (16x16, 40 min)", "Expert (16x30, 99 min)" });
            _difficultyComboBox.SelectedIndex = 0;
        }

        private void LangChanger_Click(object sender, EventArgs e)
        {
            eng = !eng;
            if (eng)
            {
                _difficultyComboBox.Items.Clear();
                _difficultyComboBox.Items.AddRange(new object[] { "Beginner (9x9, 10 min)", "Amateur (16x16, 40 min)", "Expert (16x30, 99 min)" });
            }
            else
            {
                _difficultyComboBox.Items.Clear();
                _difficultyComboBox.Items.AddRange(new object[] { "Новичок (9x9, 10 мин)", "Любитель (16x16, 40 мин)", "Эксперт (16x30, 99 мин)" });
            }
            _difficultyComboBox.SelectedIndex = 0;
            _newGameButton.Text = eng ? "New Game" : "Новая игра";
            _langChangerButton.Text = eng ? "Ru" : "En";
            _viewModel = new MainViewModel(rusCondolences, rusCongratulations, engCondolences, engCongratulations, eng);
        }
    }
}
