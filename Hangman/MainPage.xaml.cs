using Microsoft.Maui.Controls;

namespace Hangman
{
    public partial class MainPage : ContentPage
    {
        private class WordEntry
        {
            public string Word { get; }
            public string Category { get; }
            public string Databank { get; }

            public WordEntry(string word, string category, string databank)
            {
                Word = word;
                Category = category;
                Databank = databank;
            }
        }

        private readonly Dictionary<string, List<WordEntry>> wordBank = new()
        {
            ["Sith Lords"] = new()
            {
                new WordEntry("vader", "Sith Lords", "The Dark Lord of the Sith and heir to the Empire"),
                new WordEntry("palpatine", "Sith Lords", "The Emperor who secretly ruled the galaxy"),
                new WordEntry("sidious", "Sith Lords", "Darth Sidious, the ultimate Sith master"),
                new WordEntry("maul", "Sith Lords", "Zabrak Sith wielding a double-bladed lightsaber"),
                new WordEntry("tyrannus", "Sith Lords", "Darth Tyrannus, once known as Count Dooku"),
                new WordEntry("dooku", "Sith Lords", "A fallen Jedi Master and Separatist leader"),
                new WordEntry("revan", "Sith Lords", "A legendary knight who fell to the dark side"),
                new WordEntry("bane", "Sith Lords", "Founder of the Rule of Two"),
                new WordEntry("kylo", "Sith Lords", "Kylo Ren, master of the Knights of Ren"),
                new WordEntry("malgus", "Sith Lords", "A brute Sith Lord of the Old Republic era")
            },
            ["Jedi Masters"] = new()
            {
                new WordEntry("yoda", "Jedi Masters", "Grand Master of the Jedi Order, wisest of them all"),
                new WordEntry("obiwan", "Jedi Masters", "Obi-Wan Kenobi, the Negotiator"),
                new WordEntry("kenobi", "Jedi Masters", "Obi-Wan Kenobi, general of the Republic army"),
                new WordEntry("luke", "Jedi Masters", "Luke Skywalker, hero of the Rebellion"),
                new WordEntry("anakin", "Jedi Masters", "The Chosen One, father of the Skywalkers"),
                new WordEntry("quigon", "Jedi Masters", "Qui-Gon Jinn, who discovered Anakin on Tatooine"),
                new WordEntry("windu", "Jedi Masters", "Mace Windu, master of the Vaapad form"),
                new WordEntry("ahsoka", "Jedi Masters", "Anakin's Padawan who became a Rebel commander"),
                new WordEntry("rey", "Jedi Masters", "A scavenger from Jakku who became a Jedi"),
                new WordEntry("jinn", "Jedi Masters", "Qui-Gon Jinn, a maverick Jedi Master")
            },
            ["Ships & Vehicles"] = new()
            {
                new WordEntry("falcon", "Ships & Vehicles", "The Millennium Falcon, a smuggling legend"),
                new WordEntry("xwing", "Ships & Vehicles", "Rebel starfighter that destroyed the Death Star"),
                new WordEntry("tiefighter", "Ships & Vehicles", "The Empire's iconic starfighter"),
                new WordEntry("destroyer", "Ships & Vehicles", "Imperial Star Destroyer, symbol of the fleet"),
                new WordEntry("bomber", "Ships & Vehicles", "Rebel Y-wing bomber used in raids"),
                new WordEntry("interceptor", "Ships & Vehicles", "TIE Interceptor, faster than the standard TIE"),
                new WordEntry("speeder", "Ships & Vehicles", "Landspeeder cruised across the Tatooine dunes"),
                new WordEntry("shuttle", "Ships & Vehicles", "Lambda-class shuttle used by the Empire"),
                new WordEntry("cruiser", "Ships & Vehicles", "Capital ship that leads a battle group"),
                new WordEntry("deathstar", "Ships & Vehicles", "The Empire's planet-destroying superweapon")
            },
            ["Planets & Aliens"] = new()
            {
                new WordEntry("tatooine", "Planets & Aliens", "Desert planet and home of Luke Skywalker"),
                new WordEntry("coruscant", "Planets & Aliens", "The city-planet capital of the Republic"),
                new WordEntry("naboo", "Planets & Aliens", "Homeworld of Padmé Amidala and the Gungans"),
                new WordEntry("hoth", "Planets & Aliens", "Ice planet harboring a secret Rebel base"),
                new WordEntry("endor", "Planets & Aliens", "Forested moon inhabited by the Ewoks"),
                new WordEntry("dagobah", "Planets & Aliens", "Swamp planet where Yoda hid in exile"),
                new WordEntry("mustafar", "Planets & Aliens", "Volcanic world where Anakin was defeated"),
                new WordEntry("jakku", "Planets & Aliens", "Desert planet where scavenger Rey made her home"),
                new WordEntry("ewok", "Planets & Aliens", "Furry creatures of the Endor forest"),
                new WordEntry("wookiee", "Planets & Aliens", "Chewbacca's tall and loyal furry species")
            },
            ["Droids & Weapons"] = new()
            {
                new WordEntry("lightsaber", "Droids & Weapons", "The elegant weapon of a Jedi Knight"),
                new WordEntry("astromech", "Droids & Weapons", "A mechanic droid that keeps starships flying"),
                new WordEntry("r2d2", "Droids & Weapons", "The heroic astromech droid R2-D2"),
                new WordEntry("c3po", "Droids & Weapons", "A protocol droid fluent in six million languages"),
                new WordEntry("bb8", "Droids & Weapons", "The spherical astromech droid of Rey"),
                new WordEntry("blaster", "Droids & Weapons", "Standard range weapon of stormtroopers"),
                new WordEntry("kyber", "Droids & Weapons", "Crystal at the heart of every lightsaber"),
                new WordEntry("cortosis", "Droids & Weapons", "Rare metal that can resist a lightsaber"),
                new WordEntry("probe", "Droids & Weapons", "Imperial spy droid that uncovered the Hoth base"),
                new WordEntry("droid", "Droids & Weapons", "An autonomous robot used across the galaxy")
            }
        };

        private readonly List<WordEntry> allWords = new List<WordEntry>();
        private readonly List<Button> letterButtons = new List<Button>();

        private string answer = "";
        private WordEntry? currentEntry;
        private readonly List<char> guessed = new List<char>();
        private int mistakes = 0;
        private int maxWrong = 6;
        private int hintsUsed = 0;
        private const int maxHints = 3;

        public MainPage()
        {
            InitializeComponent();
            foreach (var list in wordBank.Values)
            {
                allWords.AddRange(list);
            }
            CollectLetterButtons();
            CategoryPicker.ItemsSource = wordBank.Keys.ToList();
            CategoryPicker.SelectedIndex = 0;
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
            ResetHints();
        }

        private string PickWord()
        {
            string category = CategoryPicker.SelectedIndex >= 0
                ? (string)CategoryPicker.SelectedItem
                : wordBank.Keys.First();

            var pool = wordBank.ContainsKey(category) ? wordBank[category] : allWords;
            currentEntry = pool[new Random().Next(0, pool.Count)];
            return currentEntry.Word;
        }

        private void CategoryPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetGame();
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

        private void ResetHints()
        {
            hintsUsed = 0;
            HintCountText.Text = "0 / 3";
            HintProgress.Progress = 0;
            HintTitle.Text = "NO HINTS USED";
            HintTitle.TextColor = Color.FromArgb("#5D367E");
            HintText.Text = "Use a hint to reveal intel about the word.";
            HintText.TextColor = Color.FromArgb("#696E77");
            HintButton.IsEnabled = true;
        }

        private void GiveHint()
        {
            if (hintsUsed >= maxHints || string.IsNullOrEmpty(answer))
            {
                return;
            }

            hintsUsed++;
            int remaining = maxHints - hintsUsed;
            HintCountText.Text = $"{hintsUsed} / 3";
            HintProgress.Progress = (double)hintsUsed / maxHints;

            if (hintsUsed == 1)
            {
                string databank = currentEntry?.Databank ?? "The Dark Side holds the secret.";
                HintTitle.Text = "FIRST WHISPER";
                HintTitle.TextColor = Color.FromArgb("#5D367E");
                HintText.Text = $"It is a {currentEntry?.Category}.\nAsk: {databank}?";
                HintText.TextColor = Color.FromArgb("#B78BFF");
                GameMessage.Text = "The Force whispers a faint clue...";
                GameMessage.TextColor = Color.FromArgb("#8A4DFF");
            }
            else if (hintsUsed == 2)
            {
                string category = currentEntry?.Category ?? "Unknown";
                string databank = currentEntry?.Databank ?? "The Dark Side holds the secret.";
                HintTitle.Text = "DEEPER INTEL";
                HintTitle.TextColor = Color.FromArgb("#8A4DFF");
                HintText.Text = $"This belongs to the realm of {category}.\n{databank}.";
                HintText.TextColor = Color.FromArgb("#B78BFF");
                GameMessage.Text = "The databank grows clearer...";
                GameMessage.TextColor = Color.FromArgb("#8A4DFF");
            }
            else
            {
                HintTitle.Text = "FULL DOSSIER";
                HintTitle.TextColor = Color.FromArgb("#FFD34D");
                HintText.Text =
                    $"The scroll is unsealed.\n\n{currentEntry?.Databank}.";
                HintText.TextColor = Color.FromArgb("#FFD34D");
                GameMessage.Text = "The full dossier is open. Only you can break the code.";
                GameMessage.TextColor = Color.FromArgb("#FFD34D");
            }

            if (remaining == 0)
            {
                HintButton.IsEnabled = false;
            }
        }

        private void UpdateVaderState()
        {
            int revealed = answer.Count(c => guessed.IndexOf(c) >= 0);
            double progress = answer.Length == 0 ? 0 : (double)revealed / answer.Length;

            if (revealed == answer.Length && answer.Length > 0)
            {
                VaderImage.Source = "defeat.png";
            }
            else if (mistakes >= maxWrong)
            {
                VaderImage.Source = "injured.png";
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

        private void HintButton_Clicked(object sender, EventArgs e)
        {
            GiveHint();
        }
    }
}
