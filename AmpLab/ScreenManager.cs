using System;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;

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

    if (SelectedConfiguration == "CS")
    {
        var inputRG1 = new TextBox { Left = (mainForm.Width / 2) - 110, Top = (mainForm.Height / 2) - 60, Width = 100 };
        var inputRG2 = new TextBox { Left = (mainForm.Width / 2) + 10, Top = (mainForm.Height / 2) - 60, Width = 100 };
        var inputRD = new TextBox { Left = (mainForm.Width / 2) - 110, Top = (mainForm.Height / 2) - 20, Width = 100 };
        var inputRS = new TextBox { Left = (mainForm.Width / 2) + 10, Top = (mainForm.Height / 2) - 20, Width = 100 };
        var inputCG = new TextBox { Left = (mainForm.Width / 2) - 110, Top = (mainForm.Height / 2) + 20, Width = 100 };
        var inputCS = new TextBox { Left = (mainForm.Width / 2) + 10, Top = (mainForm.Height / 2) + 20, Width = 100 };
        var inputCD = new TextBox { Left = (mainForm.Width / 2) - 110, Top = (mainForm.Height / 2) + 60, Width = 100 };

        int buttonWidth = 300, buttonHeight = 100, spacing = 20;
        int startX = (mainForm.Width - ((buttonWidth * 3) + (spacing * 2))) / 2;
        int startY = mainForm.Height - buttonHeight - 50;

        var simulateButton = ButtonFactory.CreateCustomButton("../../../images/symulacja_button.png", startX, startY, buttonWidth, buttonHeight);
        var helpButton = ButtonFactory.CreateCustomButton("../../../images/podrecznik_button.png", startX + buttonWidth + spacing, startY, buttonWidth, buttonHeight);
        var notesButton = ButtonFactory.CreateCustomButton("../../../images/notatki_button.png", startX + (buttonWidth + spacing) * 2, startY, buttonWidth, buttonHeight);

        simulateButton.Click += (sender, e) => Simulate(inputRG1.Text, inputRG2.Text, inputRD.Text, inputRS.Text, inputCG.Text, inputCS.Text, inputCD.Text);
        helpButton.Click += (sender, e) => MessageBox.Show("Pomoc: tutaj znajdziesz wskazówki!", "Pomoc");
        notesButton.Click += (sender, e) => MessageBox.Show($"Ostatni wynik: {lastSimulationResult:F2}", "Notatki");

        mainForm.Controls.AddRange(new Control[] { inputRG1, inputRG2, inputRD, inputRS, inputCG, inputCS, inputCD, simulateButton, helpButton, notesButton });
    }
    else
    {
        // Existing code for other configurations
    }
}

        private double lastSimulationResult;

        private void Simulate(string inputRG1, string inputRG2, string inputRD, string inputRS, string inputCG, string inputCS, string inputCD)
        {
            input1Value = inputRG1;
            input2Value = inputRG2;

            var culture = CultureInfo.InvariantCulture.Clone() as CultureInfo;
            culture.NumberFormat.NumberDecimalSeparator = ",";

            double.TryParse(inputRG1, NumberStyles.Any, culture, out double RG1);
            double.TryParse(inputRG2, NumberStyles.Any, culture, out double RG2);
            double.TryParse(inputRD, NumberStyles.Any, culture, out double RD);
            double.TryParse(inputRS, NumberStyles.Any, culture, out double RS);
            double.TryParse(inputCG, NumberStyles.Any, culture, out double CG);
            double.TryParse(inputCS, NumberStyles.Any, culture, out double CS);
            double.TryParse(inputCD, NumberStyles.Any, culture, out double CD);

            var simulationManager = new SimulationManager(this);
            var results = simulationManager.Simulate(RG1 * 1000, RG2 * 1000, RD * 1000, RS * 1000, CG / 1000000000, CS / 1000000000, CD / 1000000000);

            bool isInvalid = results[5] <= 0; // Update this condition based on your calculations
            results = results.Take(5).ToArray();
            simulationManager.ShowSimulationResult(results, isInvalid,
                returnToProject: () => InitializeProjectScreen(),
                submitProject: () => ShowFinalScreen(results));
        }

        private void ShowFinalScreen(double[] results)
        {
            mainForm.Controls.Clear();
            mainForm.Text = "AmpLab: Sztuka Analogów - Ocena";

            double difference = Math.Abs(TargetValue - results[0]); // Example calculation, update as needed

            var finalMessage = new Label
            {
                Text = $"Twoje wyniki to: {string.Join(", ", results.Select(r => r.ToString("F2")))}\n" +
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