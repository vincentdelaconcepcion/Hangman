using Microsoft.Maui.Controls;

namespace Hangman
{
    public partial class MainPage : ContentPage
    {


        public MainPage()
        {
            InitializeComponent();
            Letters.AddRange("abcdefghijklmnopqrstuvwxyz");
            BindingContext = this;
            PickWord();
            CalculateWord(answer, guessed);
        }

        #region UI Properties
        public string Spotlight
        {
            get => spotlight;
            set
            {
                spotlight = value;
                OnPropertyChanged();
            }
        }

        public List<char> Letters
        {
            get => letters;
            set
            {
                letters = value;
                OnPropertyChanged();
            }
        }

        public string Message
        {
            get => message;
            set
            {
                message = value;
                OnPropertyChanged();
            }
        }

        public string GameStatus
        {
            get => gameStatus;
            set
            {
                gameStatus = value;
                OnPropertyChanged();
            }
        }
        public string CurrentImage
        {
            get => currentImage;
            set
            {
                currentImage = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Fields
        List<string> words = new List<string>()
            {
                "python",
                "javascript",
                "maui",
                "csharp",
                "mongodb",
                "sql",
                "xaml",
                "word",
                "excel",
                "powerpoint",
                "code",
                "hotreload",
                "snippets"
            };

        string answer = "";
        private string spotlight = String.Empty;
        List<char> guessed = new List<char>();
        private List<char> letters = new List<char>();
        private string message = String.Empty;
        int mistakes = 0;
        int maxWrong = 6;
        private string gameStatus = String.Empty;
        private string currentImage = "dotnet_bot.png";
        #endregion

        #region Game Engine
        private void PickWord()
        {
            answer = words[new Random().Next(0, words.Count)];
        }

        private void CalculateWord(string answer, List<char> guessed)
        {
            var temp =
                answer.Select(x => (guessed.IndexOf(x) >= 0 ? x : '_'))
                .ToArray();
            Spotlight = string.Join(' ', temp);
        }

        private void HandleGuess(char letter)
        {
            if (guessed.IndexOf(letter) == -1)
            {
                guessed.Add(letter);
            }
            if (answer.IndexOf(letter) >= 0)
            {
                CalculateWord(answer, guessed);
                CheckIfGameWon();
            }
            else if (answer.IndexOf(letter) == -1)
            {
                mistakes++;
                UpdateStatus();
                CheckIfGameLost();
                CurrentImage = "dotnet_bot.png";
            }
        }

        private void CheckIfGameLost()
        {
            if (mistakes == maxWrong)
            {
                Message = "You Lost!!";
                DisableLetters();
            }
        }

        private void DisableLetters()
        {
            foreach (var children in LettersContainer.Children)
            {
                var btn = children as Button;
                if (btn != null)
                {
                    btn.IsEnabled = false;
                }
            }
        }

        private void EnableLetters()
        {
            foreach (var children in LettersContainer.Children)
            {
                var btn = children as Button;
                if (btn != null)
                {
                    btn.IsEnabled = true;
                }
            }
        }

        private void CheckIfGameWon()
        {
            if (Spotlight.Replace(" ", "") == answer)
            {
                Message = "You win!";
                DisableLetters();
            }
        }

        private void UpdateStatus()
        {
            GameStatus = $"Errors: {mistakes} of {maxWrong}";
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            var btn = sender as Button;
            if (btn != null)
            {
                var letter = btn.Text;
                btn.IsEnabled = false;
                HandleGuess(letter[0]);
            }
        }

        private void Reset_Clicked(object sender, EventArgs e)
        {
            mistakes = 0;
            guessed = new List<char>();
            CurrentImage = "dotnet_bot.png";
            PickWord();
            CalculateWord(answer, guessed);
            Message = "";
            UpdateStatus();
            EnableLetters();
        }
        #endregion
    }
    
}
