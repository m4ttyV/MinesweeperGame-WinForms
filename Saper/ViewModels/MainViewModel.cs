using Saper.Models;
using System.Windows.Input;

namespace Saper.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private MinesweeperGame _game;
        private GameSettings _currentSettings;
        private Random _random = new Random();
        public Dictionary<int, string> rusCongratulations { get; set; }
        public Dictionary<int, string> rusCondolences { get; set; }
        public Dictionary<int, string> engCongratulations { get; set; }
        public Dictionary<int, string> engCondolences { get; set; }
        public bool eng { get; set; }
        public GameSettings CurrentSettings
        {
            get => _currentSettings;
            set => SetProperty(ref _currentSettings, value);
        }

        public MinesweeperGame Game
        {
            get => _game;
            set => SetProperty(ref _game, value);
        }

        public ICommand NewGameCommand { get; }
        public ICommand CellClickCommand { get; }
        public ICommand CellRightClickCommand { get; }
        public ICommand ChangeDifficultyCommand { get; }

        // Настройки сложности
        public GameSettings BeginnerSettings => new GameSettings(9, 9, 10);
        public GameSettings IntermediateSettings => new GameSettings(16, 16, 40);
        public GameSettings ExpertSettings => new GameSettings(16, 30, 99);
        public void OnCellLeftClick(int row, int col)
        {
            Game?.RevealCell(row, col);
            OnPropertyChanged(nameof(Game));
        }

        public void OnCellRightClick(int row, int col)
        {
            Game?.ToggleFlag(row, col);
            OnPropertyChanged(nameof(Game));
        }
        public void OnNewGame()
        {
            InitializeNewGame();
        }

        public void OnChangeDifficulty(GameSettings settings)
        {
            CurrentSettings = settings;
            InitializeNewGame();
        }

        public MainViewModel(Dictionary<int, string> rusCondolences, Dictionary<int, string> rusCongratulations, 
            Dictionary<int, string> engCondolences, Dictionary<int, string> engCongratulations, bool eng)
        {
            CurrentSettings = BeginnerSettings;
            InitializeNewGame();

            NewGameCommand = new RelayCommand(InitializeNewGame);
            CellClickCommand = new RelayCommand<int, int>(RevealCell);
            CellRightClickCommand = new RelayCommand<int, int>(ToggleFlag);
            ChangeDifficultyCommand = new RelayCommand<GameSettings>(ChangeDifficulty);
            this.rusCondolences = rusCondolences;
            this.rusCongratulations = rusCongratulations;
            this.engCondolences = engCondolences;
            this.engCongratulations = engCongratulations;
            this.eng = eng;
        }

        private void InitializeNewGame()
        {
            Game = new MinesweeperGame(CurrentSettings);

            // Подписываемся на события игры
            Game.CellRevealed += (cell) => OnPropertyChanged(nameof(Game));
            Game.CellFlagged += (cell) => OnPropertyChanged(nameof(Game));
            Game.GameWon += OnGameWon;
            Game.GameLost += OnGameLost;
        }

        private void RevealCell(int row, int col)
        {
            Game?.RevealCell(row, col);
        }

        private void ToggleFlag(int row, int col)
        {
            Game?.ToggleFlag(row, col);
        }

        private void ChangeDifficulty(GameSettings settings)
        {
            CurrentSettings = settings;
            InitializeNewGame();
        }

        private void OnCellRevealed(Cell cell)
        {
            // Здесь можно добавить логику обновления UI
            OnPropertyChanged(nameof(Game));
        }

        private void OnCellFlagged(Cell cell)
        {
            OnPropertyChanged(nameof(Game));
        }

        private void OnGameWon()
        {
            // Логика победы
            string message;
            var form1 = new Form1();
            if (eng)
            {
                int tmp = engCongratulations.Count();
                message = engCongratulations[_random.Next(0, engCongratulations.Count())];
            }
            else
            {
                int tmp = rusCongratulations.Count();
                message = rusCongratulations[_random.Next(0, rusCongratulations.Count())];
            }
            MessageBox.Show(message);
        }

        private void OnGameLost()
        {
            string message;
            if (eng)
            {
                int tmp = engCondolences.Count();
                message = engCondolences[_random.Next(tmp)];
            }
            else
            {
                int tmp = rusCondolences.Count();
                message = rusCondolences[_random.Next(tmp)];
            }
            MessageBox.Show(message);
        }
    }
}
