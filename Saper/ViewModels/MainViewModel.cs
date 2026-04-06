using Saper.Models;
using System.Windows.Input;

namespace Saper.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private MinesweeperGame _game;
        private GameSettings _currentSettings;
        private readonly Random _random = new();

        public Dictionary<int, string> rusCongratulations { get; set; }
        public Dictionary<int, string> rusCondolences { get; set; }
        public Dictionary<int, string> engCongratulations { get; set; }
        public Dictionary<int, string> engCondolences { get; set; }
        public bool eng { get; private set; }

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

        public GameSettings BeginnerSettings => new(9, 9, 10);
        public GameSettings IntermediateSettings => new(16, 16, 40);
        public GameSettings ExpertSettings => new(16, 30, 99);

        public MainViewModel(
            Dictionary<int, string> rusCondolences,
            Dictionary<int, string> rusCongratulations,
            Dictionary<int, string> engCondolences,
            Dictionary<int, string> engCongratulations,
            bool eng)
        {
            this.rusCondolences = rusCondolences;
            this.rusCongratulations = rusCongratulations;
            this.engCondolences = engCondolences;
            this.engCongratulations = engCongratulations;
            this.eng = eng;

            CurrentSettings = BeginnerSettings;
            InitializeNewGame();

            NewGameCommand = new RelayCommand(InitializeNewGame);
            CellClickCommand = new RelayCommand<int, int>(RevealCell);
            CellRightClickCommand = new RelayCommand<int, int>(ToggleFlag);
            ChangeDifficultyCommand = new RelayCommand<GameSettings>(ChangeDifficulty);
        }

        public void SetLanguage(bool isEnglish)
        {
            eng = isEnglish;
        }

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

        private void InitializeNewGame()
        {
            Game = new MinesweeperGame(CurrentSettings);
            Game.CellRevealed += _ => OnPropertyChanged(nameof(Game));
            Game.CellFlagged += _ => OnPropertyChanged(nameof(Game));
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

        private void OnGameWon()
        {
            string title = eng ? "Victory" : "Победа";
            string message = eng
                ? GetRandomMessage(engCongratulations, "You cleared the whole field!")
                : GetRandomMessage(rusCongratulations, "Ты очистил всё поле!");

            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OnGameLost()
        {
            string title = eng ? "Boom" : "Бум";
            string message = eng
                ? GetRandomMessage(engCondolences, "That was a mine. Try again!")
                : GetRandomMessage(rusCondolences, "Это была мина. Попробуй ещё раз!");

            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private string GetRandomMessage(Dictionary<int, string> source, string fallback)
        {
            if (source == null || source.Count == 0)
            {
                return fallback;
            }

            return source[_random.Next(source.Count)];
        }
    }
}
