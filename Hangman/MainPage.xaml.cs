using Microsoft.Maui.Controls;

namespace Hangman
{
    public partial class MainPage : ContentPage
    {
        private readonly List<string> words = new List<string>()
        {
            "python", "javascript", "maui", "csharp", "mongodb",
            "sql", "xaml", "word", "excel", "powerpoint",
            "code", "hotreload", "snippets", "android", "vader"
        };

        private readonly List<Button> letterButtons = new List<Button>();

        private string answer = "";
        private readonly List<char> guessed = new List<char>();
        private int mistakes = 0;
        private int maxWrong = 6;

        public MainPage()
        {
            InitializeComponent();
            CollectLetterButtons();
            ResetGame();
        }

        private void CollectLetterButtons()
        {
            letterButtons.Clear();
            foreach (char c in "ABCDEFGHIJKLMNOPQRSTUVWXYZ")
            {
                var btn = this.FindByName<Button>($"Btn{c}");
                if (btn != null)
                {
                    letterButtons.Add(btn);
                }
            }
        }

        private void ResetGame()
        {
            mistakes = 0;
            guessed.Clear();
            answer = PickWord();

            UpdateWordDisplay();
            UpdateStatus();

            GameMessage.Text = "Choose a letter. The Dark Lord is watching.";
            GameMessage.TextColor = Color.FromArgb("#999EA8");
            StatusText.Text = "READY";
            StatusText.TextColor = Color.FromArgb("#FF3A42");
            StatusBadge.BackgroundColor = Color.FromArgb("#211015");
            StatusBadge.Stroke = Color.FromArgb("#57171B");
            VaderStatus.Text = "DARTH VADER";

            EnableLetters();
            UpdateVaderState();
        }

        private string PickWord()
        {
            return words[new Random().Next(0, words.Count)];
        }

        private void UpdateWordDisplay()
        {
            var temp =
                answer.Select(x => guessed.IndexOf(x) >= 0 ? x : '_')
                .ToArray();

            WordLabel.Text = string.Join(" ", temp);
            WordLabel.CharacterSpacing = 8;

            int correctCount = answer.Count(c => guessed.IndexOf(c) >= 0);
            double progress = answer.Length == 0 ? 0 : (double)correctCount / answer.Length;
            WordProgress.Progress = progress;
            ProgressText.Text = $"{Math.Round(progress * 100)}%";
        }

        private void UpdateStatus()
        {
            int remaining = maxWrong - mistakes;
            WrongCounter.Text = $"{mistakes} / {maxWrong}";
            MistakeText.Text = $"{remaining} MISTAKES LEFT";
            MistakeBar.Progress = (double)mistakes / maxWrong;
        }

        private void UpdateVaderState()
        {
            int revealed = answer.Count(c => guessed.IndexOf(c) >= 0);
            double progress = answer.Length == 0 ? 0 : (double)revealed / answer.Length;

            if (revealed == answer.Length && answer.Length > 0)
            {
                VaderImage.Source = "vaderlogo.png";
            }
            else if (mistakes >= maxWrong)
            {
                VaderImage.Source = "defeat.png";
            }
            else if (mistakes >= 3)
            {
                VaderImage.Source = "choke.png";
            }
            else if (progress >= 0.40 && progress <= 0.50)
            {
                VaderImage.Source = "injured.png";
            }
            else
            {
                VaderImage.Source = "ready.png";
            }
        }

        private void HandleGuess(char letter)
        {
            if (guessed.IndexOf(letter) >= 0)
            {
                return;
            }

            guessed.Add(letter);

            if (answer.IndexOf(letter) < 0)
            {
                mistakes++;
                GameMessage.Text = $"'{char.ToUpper(letter)}' is not in the transmission.";
                GameMessage.TextColor = Color.FromArgb("#A7ABB3");
            }

            UpdateStatus();
            UpdateWordDisplay();
            UpdateVaderState();

            if (CheckIfGameWon())
            {
                return;
            }

            CheckIfGameLost();
        }

        private bool CheckIfGameWon()
        {
            if (string.Join("", WordLabel.Text.Split(' ')) == answer)
            {
                GameMessage.Text = "The transmission is decoded. You win!";
                GameMessage.TextColor = Color.FromArgb("#FFD34D");
                StatusText.Text = "VICTORY";
                StatusText.TextColor = Color.FromArgb("#4DFF8A");
                StatusBadge.BackgroundColor = Color.FromArgb("#10221A");
                StatusBadge.Stroke = Color.FromArgb("#2D6B46");
                VaderStatus.Text = "VADER IS DEFEATED";
                UpdateVaderState();
                DisableLetters();
                return true;
            }
            return false;
        }

        private bool CheckIfGameLost()
        {
            if (mistakes >= maxWrong)
            {
                GameMessage.Text = $"You lost! The word was \"{answer.ToUpper()}\".";
                GameMessage.TextColor = Color.FromArgb("#FF3A42");
                StatusText.Text = "DEFEATED";
                StatusText.TextColor = Color.FromArgb("#FF3A42");
                StatusBadge.BackgroundColor = Color.FromArgb("#211015");
                StatusBadge.Stroke = Color.FromArgb("#57171B");
                VaderStatus.Text = "VADER HAS WON";
                UpdateVaderState();
                DisableLetters();
                return true;
            }
            return false;
        }

        private void DisableLetters()
        {
            foreach (var btn in letterButtons)
            {
                btn.IsEnabled = false;
            }
        }

        private void EnableLetters()
        {
            foreach (var btn in letterButtons)
            {
                btn.IsEnabled = true;
            }
        }

        private void LetterButton_Clicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.Text.Length > 0)
            {
                btn.IsEnabled = false;
                HandleGuess(char.ToLower(btn.Text[0]));
            }
        }

        private void ResetButton_Clicked(object sender, EventArgs e)
        {
            ResetGame();
        }
    }
}
