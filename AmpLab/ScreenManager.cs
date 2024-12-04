using System;
using System.Drawing;
using System.Windows.Forms;

namespace AmpLab
{
    public class ScreenManager
    {
        private Form mainForm;
        private Random random = new Random();

        public double TargetValue { get; private set; } // Wartość zadana
        public string SelectedConfiguration { get; private set; } // Wybrana konfiguracja

        private string input1Value;
        private string input2Value;

        public ScreenManager(Form form)
        {
            mainForm = form;
        }

        // Ekran startowy
        public void InitializeStartScreen()
        {
            mainForm.Controls.Clear();
            mainForm.Text = "AmpLab: Sztuka Analogów";
            mainForm.BackgroundImage = Image.FromFile("../../../images/Background_difficulty.png");
            mainForm.BackgroundImageLayout = ImageLayout.Stretch;

            int buttonWidth = 300, buttonHeight = 300, spacing = 20;
            int startX = (mainForm.Width - ((buttonWidth * 3) + (spacing * 2))) / 2;
            int startY = (mainForm.Height - buttonHeight) / 2;

            var easyButton = ButtonFactory.CreateCustomButton("../../../images/path_to_easy_image.png", startX, startY, buttonWidth, buttonHeight);
            var mediumButton = ButtonFactory.CreateCustomButton("../../../images/path_to_medium_image.png", startX + buttonWidth + spacing, startY, buttonWidth, buttonHeight);
            var hardButton = ButtonFactory.CreateCustomButton("../../../images/path_to_hard_image.png", startX + (buttonWidth + spacing) * 2, startY, buttonWidth, buttonHeight);

            easyButton.Click += (sender, e) => SelectDifficulty("Easy");
            mediumButton.Click += (sender, e) => SelectDifficulty("Medium");
            hardButton.Click += (sender, e) => SelectDifficulty("Hard");

            mainForm.Controls.AddRange(new[] { easyButton, mediumButton, hardButton });
        }

        private void SelectDifficulty(string difficulty)
        {
            TargetValue = difficulty switch
            {
                "Easy" => random.Next(10, 20),
                "Medium" => random.Next(20, 50),
                "Hard" => random.Next(50, 100),
                _ => 0
            };

            MessageBox.Show($"Wartość zadana: {TargetValue:F2}", "Wymagania");
            InitializeConfigurationScreen();
        }

        // Ekran konfiguracji
        public void InitializeConfigurationScreen()
        {
            mainForm.Controls.Clear();
            mainForm.Text = "AmpLab: Sztuka Analogów - Konfiguracja";
            mainForm.BackgroundImage = Image.FromFile("../../../images/Background.png");
            mainForm.BackgroundImageLayout = ImageLayout.Stretch;

            int buttonWidth = 300, buttonHeight = 300, spacing = 20;
            int startX = (mainForm.Width - ((buttonWidth * 3) + (spacing * 2))) / 2;
            int startY = (mainForm.Height - buttonHeight) / 2;

            var csButton = ButtonFactory.CreateCustomButton("../../../images/cs_button.png", startX, startY, buttonWidth, buttonHeight);
            var cgButton = ButtonFactory.CreateCustomButton("../../../images/cg_button.png", startX + buttonWidth + spacing, startY, buttonWidth, buttonHeight);
            var cdButton = ButtonFactory.CreateCustomButton("../../../images/cd_button.png", startX + (buttonWidth + spacing) * 2, startY, buttonWidth, buttonHeight);

            csButton.Click += (sender, e) => SelectConfiguration("CS");
            cgButton.Click += (sender, e) => SelectConfiguration("CG");
            cdButton.Click += (sender, e) => SelectConfiguration("CD");

            mainForm.Controls.AddRange(new[] { csButton, cgButton, cdButton });
        }

        private void SelectConfiguration(string configuration)
        {
            SelectedConfiguration = configuration;
            InitializeProjectScreen();
        }

        // Ekran projektowy
        public void InitializeProjectScreen()
        {
            mainForm.Controls.Clear();
            mainForm.Text = "AmpLab: Sztuka Analogów - Stół Projektowy";
            mainForm.BackgroundImage = Image.FromFile("../../../images/Background.png");
            mainForm.BackgroundImageLayout = ImageLayout.Stretch;

            var input1 = new TextBox { Left = (mainForm.Width / 2) - 110, Top = (mainForm.Height / 2) - 20, Width = 100, Text = input1Value };
            var input2 = new TextBox { Left = (mainForm.Width / 2) + 10, Top = (mainForm.Height / 2) - 20, Width = 100, Text = input2Value };

            int buttonWidth = 300, buttonHeight = 100, spacing = 20;
            int startX = (mainForm.Width - ((buttonWidth * 3) + (spacing * 2))) / 2;
            int startY = mainForm.Height - buttonHeight - 50;

            var simulateButton = ButtonFactory.CreateCustomButton("../../../images/symulacja_button.png", startX, startY, buttonWidth, buttonHeight);
            var helpButton = ButtonFactory.CreateCustomButton("../../../images/podrecznik_button.png", startX + buttonWidth + spacing, startY, buttonWidth, buttonHeight);
            var notesButton = ButtonFactory.CreateCustomButton("../../../images/notatki_button.png", startX + (buttonWidth + spacing) * 2, startY, buttonWidth, buttonHeight);

            simulateButton.Click += (sender, e) => Simulate(input1.Text, input2.Text);
            helpButton.Click += (sender, e) => MessageBox.Show("Pomoc: tutaj znajdziesz wskazówki!", "Pomoc");
            notesButton.Click += (sender, e) => MessageBox.Show($"Ostatni wynik: {lastSimulationResult:F2}", "Notatki");

            mainForm.Controls.AddRange(new Control[] { input1, input2, simulateButton, helpButton, notesButton });
        }

        private double lastSimulationResult;

        private void Simulate(string input1, string input2)
        {
            input1Value = input1;
            input2Value = input2;

            double.TryParse(input1, out double value1);
            double.TryParse(input2, out double value2);

            var simulationManager = new SimulationManager(this);
            double result = simulationManager.Simulate(value1, value2);

            bool isInvalid = result <= 0;
            simulationManager.ShowSimulationResult(result, isInvalid,
                returnToProject: () => InitializeProjectScreen(),
                submitProject: () => ShowFinalScreen(result));
        }

        private void ShowFinalScreen(double result)
        {
            mainForm.Controls.Clear();
            mainForm.Text = "AmpLab: Sztuka Analogów - Ocena";

            double difference = Math.Abs(TargetValue - result);

            var finalMessage = new Label
            {
                Text = $"Twój wynik to: {result:F2}\n" +
                       $"Wartość zadana: {TargetValue:F2}\n" +
                       $"Różnica: {difference:F2}\nDziękujemy za projekt!",
                Left = 50,
                Top = 50,
                AutoSize = true
            };

            var exitButton = new Button
            {
                Text = "Zakończ",
                Left = 50,
                Top = 150,
                Width = 100
            };
            exitButton.Click += (s, e) => InitializeStartScreen();

            mainForm.Controls.AddRange(new Control[] { finalMessage, exitButton });
        }
    }
}