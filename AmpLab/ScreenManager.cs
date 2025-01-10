using System.Globalization;
using Timer = System.Windows.Forms.Timer;

namespace AmpLab;

public class ScreenManager
{
    private Timer projectTimer;
    private bool timerRunning;
    private Label timerLabel;
    private int elapsedSeconds;
    public string InputRG1 { get; set; }
    public string InputRG2 { get; set; }
    public string InputRD { get; set; }
    public string InputRS { get; set; }
    public string InputCG { get; set; }
    public string InputCS { get; set; }
    public string InputCD { get; set; }
    private readonly Form mainForm;
    private Random random = new();

    public double[] TargetValue { get; private set; } // Wartość zadana
    public double[] expectedValues { get; private set; } // Wartości wylosowane
    public double K, Ri, Ro, fH, fL; // Wartości zadane
    public string SelectedConfiguration { get; private set; } // Wybrana konfiguracja
    public string SelectedDifficulty { get; private set; } // Wybrany poziom trudności

    private double[] lastSimulationResult = { 0, 0, 0, 0, 0 };

    public ScreenManager(Form form)
    {
        mainForm = form;
    }

    // Ekran startowy
    public void InitializeStartScreen()
    {
        mainForm.Controls.Clear();
        mainForm.MaximizeBox = false;
        mainForm.Text = "AmpLab: Sztuka Analogów";
        mainForm.BackgroundImage = Image.FromFile("../../../images/Background_difficulty.png");
        mainForm.BackgroundImageLayout = ImageLayout.Stretch;

        int buttonWidth = 300, buttonHeight = 300, spacing = 20;
        var startX = (mainForm.Width - (buttonWidth * 3 + spacing * 2)) / 2;
        var startY = (mainForm.Height - buttonHeight) / 2;

        var easyButton = ButtonFactory.CreateCustomButton("../../../images/path_to_easy_image.png", startX, startY,
            buttonWidth, buttonHeight);
        var mediumButton = ButtonFactory.CreateCustomButton("../../../images/path_to_medium_image.png",
            startX + buttonWidth + spacing, startY, buttonWidth, buttonHeight);
        var hardButton = ButtonFactory.CreateCustomButton("../../../images/path_to_hard_image.png",
            startX + (buttonWidth + spacing) * 2, startY, buttonWidth, buttonHeight);

        easyButton.Click += (sender, e) => SelectDifficulty("Easy");
        mediumButton.Click += (sender, e) => SelectDifficulty("Medium");
        hardButton.Click += (sender, e) => SelectDifficulty("Hard");

        mainForm.Controls.AddRange(easyButton, mediumButton, hardButton);
    }

    private void SelectDifficulty(string difficulty)
    {
        SelectedDifficulty = difficulty;
        TargetValue = difficulty switch
        {
            "Easy" => targetEasy(K, Ri, Ro, fH, fL),
            "Medium" => targetMedium(K, Ri, Ro, fH, fL),
            "Hard" => targetHard(K, Ri, Ro, fH, fL),
            _ => new double[] { 0, 0, 0, 0, 0 }
        };

        MessageBox.Show($"\tWartości zadane:\n\n" +
                        $"Wzmocnienie:\t{TargetValue[0]:F2} V/V\n" +
                        $"Rin:\t\t{TargetValue[1] / 1000:F2} kΩ\n" +
                        $"Rout:\t\t{TargetValue[2] / 1000:F2} kΩ\n" +
                        $"fL3dB:\t\t{TargetValue[4]:F2} Hz\n" +
                        $"fH3dB:\t\t{TargetValue[3] / 1000:F2} kHz", "Wymagania");
        InitializeConfigurationScreen();
    }

    private double[] targetEasy(double K, double Ri, double Ro, double fH, double fL)
    {
        var results = InputGenerator();

// Store results in variables
        K = results[0];
        Ri = results[1];
        Ro = results[2];
        fH = results[3];
        fL = results[4];
        return new[] { K, Ri, Ro, fH, fL };
    }

    private double[] targetMedium(double K, double Ri, double Ro, double fH, double fL)
    {
        var results = InputGenerator();
        var random = new Random();
// Store results in variables
        K = results[0] * (0.9 + random.NextDouble() * 0.2);
        Ri = results[1] * (0.9 + random.NextDouble() * 0.2);
        Ro = results[2] * (0.9 + random.NextDouble() * 0.2);
        fH = results[3] * (0.9 + random.NextDouble() * 0.2);
        fL = results[4] * (0.9 + random.NextDouble() * 0.2);
        return new[] { K, Ri, Ro, fH, fL };
    }

    private double[] targetHard(double K, double Ri, double Ro, double fH, double fL)
    {
        var results = InputGenerator();
        var random = new Random();
// Store results in variables
        K = results[0] * (0.8 + random.NextDouble() * 0.4);
        Ri = results[1] * (0.8 + random.NextDouble() * 0.4);
        Ro = results[2] * (0.8 + random.NextDouble() * 0.4);
        fH = results[3] * (0.8 + random.NextDouble() * 0.4);
        fL = results[4] * (0.8 + random.NextDouble() * 0.4);
        return new[] { K, Ri, Ro, fH, fL };
    }

    private double[] InputGenerator()
    {
        var random = new Random();
        var repeat = false;
        double[] results;
        double RG1;
        double RG2;
        double RD;
        double RS;
        double CG;
        double CS;
        double CD;
        var confGenerate = random.Next(1, 4);

        do
        {
            // Losowanie Wartości
            RG1 = Math.Round(random.NextDouble() * 990000 + 10000, 2); // range: 10 to 1000
            RG2 = Math.Round(random.NextDouble() * 790000 + 10000, 2); // range: 10 to 800
            RD = Math.Round(random.NextDouble() * 99000 + 1000, 2); // range: 1 to 100
            RS = Math.Round(random.NextDouble() * 99000 + 1000, 2); // range: 1 to 100
            CG = random.NextDouble() * 1.99e-8 + 1e-10; // range: 0.1 to 20
            CS = random.NextDouble() * 9.99e-7 + 1e-8; // range: 10 to 1000
            CD = random.NextDouble() * 4.99e-7 + 1e-9; // range: 1 to 500

// Sprawdzenie symulacją
            var simulationManager = new SimulationManager(this);
            switch (confGenerate)
            {
                case 1:
                    results = simulationManager.SimulateCS(RG1, RG2, RD, RS, CG, CS, CD);
                    break;
                case 2:
                    results = simulationManager.SimulateCG(RG1, RG2, RD, RS, CG, CS, CD);
                    break;
                case 3:
                    results = simulationManager.SimulateCD(RG1, RG2, RD, RS, CG, CS, CD);
                    break;
                default:
                    results = new double[] { 0, 0, 0, 0, 0, 0, 0 };
                    break;
            }

            repeat = Math.Round(results[0], 2) == 0 || results[5] < 0 || results[6] == 0 || results[7] == 1;
        } while (repeat);

        expectedValues = [RG1 / 1E3, RG2 / 1E3, RD / 1E3, RS / 1E3, CG * 1E9, CS * 1E9, CD * 1E9];
        K = results[0];
        Ri = results[1];
        Ro = results[2];
        fH = results[3];
        fL = results[4];
        return new[] { K, Ri, Ro, fH, fL };
    }

    // Ekran wyboru konfiguracji
    public void InitializeConfigurationScreen()
    {
        mainForm.Controls.Clear();
        mainForm.MaximizeBox = false;
        mainForm.Text = "AmpLab: Sztuka Analogów - Konfiguracja";
        mainForm.BackgroundImage = Image.FromFile("../../../images/Background.png");
        mainForm.BackgroundImageLayout = ImageLayout.Stretch;

        int buttonWidth = 300, buttonHeight = 300, spacing = 20;
        var startX = (mainForm.Width - (buttonWidth * 3 + spacing * 2)) / 2;
        var startY = (mainForm.Height - buttonHeight) / 2;

        var csButton = ButtonFactory.CreateCustomButton("../../../images/cs_button.png", startX, startY,
            buttonWidth, buttonHeight);
        var cgButton = ButtonFactory.CreateCustomButton("../../../images/cg_button.png",
            startX + buttonWidth + spacing, startY, buttonWidth, buttonHeight);
        var cdButton = ButtonFactory.CreateCustomButton("../../../images/cd_button.png",
            startX + (buttonWidth + spacing) * 2, startY, buttonWidth, buttonHeight);

        csButton.Click += (sender, e) => SelectConfiguration("CS");
        cgButton.Click += (sender, e) => SelectConfiguration("CG");
        cdButton.Click += (sender, e) => SelectConfiguration("CD");

        mainForm.Controls.AddRange(csButton, cgButton, cdButton);
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
        mainForm.MaximizeBox = false;
        mainForm.Text = "AmpLab: Sztuka Analogów - Stół Projektowy";
        mainForm.BackgroundImage = Image.FromFile("../../../images/Background_CS.png");
        mainForm.BackgroundImageLayout = ImageLayout.Stretch;
        var inputRG1 = new TextBox { Width = 80, Text = InputRG1 };
        var inputRG2 = new TextBox { Width = 80, Text = InputRG2 };
        var inputRD = new TextBox { Width = 80, Text = InputRD };
        var inputRS = new TextBox { Width = 80, Text = InputRS };
        var inputCG = new TextBox { Width = 80, Text = InputCG };
        var inputCS = new TextBox { Width = 80, Text = InputCS };
        var inputCD = new TextBox { Width = 80, Text = InputCD };
        if (SelectedConfiguration == "CD")
        {
            inputRD.Enabled = false;
            inputCD.Enabled = false;
            inputRD.Visible = false;
            inputCD.Visible = false;
        }

        switch (SelectedConfiguration)
        {
            case "CS":
                mainForm.BackgroundImage = Image.FromFile("../../../images/Background_CS.png");
                inputRG1.Left = mainForm.Width / 2 - 155;
                inputRG1.Top = mainForm.Height / 2 - 258;
                inputRG2.Left = mainForm.Width / 2 - 155;
                inputRG2.Top = mainForm.Height / 2 - 43;
                inputRD.Left = mainForm.Width / 2 - 28;
                inputRD.Top = mainForm.Height / 2 - 258;
                inputRS.Left = mainForm.Width / 2 - 28;
                inputRS.Top = mainForm.Height / 2 - 43;
                inputCG.Left = mainForm.Width / 2 - 268;
                inputCG.Top = mainForm.Height / 2 - 120;
                inputCS.Left = mainForm.Width / 2 + 142;
                inputCS.Top = mainForm.Height / 2 - 43;
                inputCD.Left = mainForm.Width / 2 + 45;
                inputCD.Top = mainForm.Height / 2 - 180;
                break;
            case "CD":
                mainForm.BackgroundImage = Image.FromFile("../../../images/Background_CD.png");
                inputRG1.Left = mainForm.Width / 2 - 155;
                inputRG1.Top = mainForm.Height / 2 - 258;
                inputRG2.Left = mainForm.Width / 2 - 155;
                inputRG2.Top = mainForm.Height / 2 - 43;
                inputRS.Left = mainForm.Width / 2 - 28;
                inputRS.Top = mainForm.Height / 2 - 43;
                inputCG.Left = mainForm.Width / 2 - 268;
                inputCG.Top = mainForm.Height / 2 - 120;
                inputCS.Left = mainForm.Width / 2 + 142;
                inputCS.Top = mainForm.Height / 2 - 129;
                break;
            case "CG":
                mainForm.BackgroundImage = Image.FromFile("../../../images/Background_CG.png");
                inputRG1.Left = mainForm.Width / 2 - 92;
                inputRG1.Top = mainForm.Height / 2 - 258;
                inputRG2.Left = mainForm.Width / 2 - 92;
                inputRG2.Top = mainForm.Height / 2 - 43;
                inputRS.Left = mainForm.Width / 2 + 37;
                inputRS.Top = mainForm.Height / 2 - 43;
                inputRD.Left = mainForm.Width / 2 + 37;
                inputRD.Top = mainForm.Height / 2 - 258;
                inputCG.Left = mainForm.Width / 2 - 230;
                inputCG.Top = mainForm.Height / 2 - 33;
                inputCS.Left = mainForm.Width / 2 - 237;
                inputCS.Top = mainForm.Height / 2 - 60;
                inputCD.Left = mainForm.Width / 2 + 80;
                inputCD.Top = mainForm.Height / 2 - 184;
                break;
            default:
                mainForm.BackgroundImage = null;
                break;
        }

        int buttonWidth = 300, buttonHeight = 100, spacing = 20;
        var startX = (mainForm.Width - (buttonWidth * 3 + spacing * 2)) / 2;
        var startY = mainForm.Height - buttonHeight - 50;

        var simulateButton = ButtonFactory.CreateCustomButton("../../../images/symulacja_button.png", startX,
            startY, buttonWidth, buttonHeight);
        var helpButton = ButtonFactory.CreateCustomButton("../../../images/podrecznik_button.png",
            startX + buttonWidth + spacing, startY, buttonWidth, buttonHeight);
        var notesButton = ButtonFactory.CreateCustomButton("../../../images/notatki_button.png",
            startX + (buttonWidth + spacing) * 2, startY, buttonWidth, buttonHeight);

        if (SelectedConfiguration == "CD")
            simulateButton.Click += (sender, e) => Simulate(inputRG1.Text, inputRG2.Text, "0", inputRS.Text,
                inputCG.Text, inputCS.Text, inputCD.Text);
        else
            simulateButton.Click += (sender, e) => Simulate(inputRG1.Text, inputRG2.Text, inputRD.Text,
                inputRS.Text, inputCG.Text, inputCS.Text, inputCD.Text);

        helpButton.Click += (sender, e) =>
        {
            double.TryParse(string.IsNullOrEmpty(InputRG1) ? "0" : InputRG1, out var RG1);
            double.TryParse(string.IsNullOrEmpty(InputRG2) ? "0" : InputRG2, out var RG2);
            double.TryParse(string.IsNullOrEmpty(InputRD) ? "0" : InputRD, out var RD);
            double.TryParse(string.IsNullOrEmpty(InputRS) ? "0" : InputRS, out var RS);
            double.TryParse(string.IsNullOrEmpty(InputCG) ? "0" : InputCG, out var CG);
            double.TryParse(string.IsNullOrEmpty(InputCS) ? "0" : InputCS, out var CS);
            double.TryParse(string.IsNullOrEmpty(InputCD) ? "0" : InputCD, out var CD);

            var helpManager = new HelpManager(
                SelectedDifficulty,
                SelectedConfiguration,
                RG1,
                RG2,
                RD,
                RS,
                CG,
                CS,
                CD,
                expectedValues
            );
            helpManager.ShowHelp();
        };
        notesButton.Click += (sender, e) => MessageBox.Show($"\tWartości zadane\tuzyskane\n\n" +
                                                            $"Wzmocnienie:\t{TargetValue[0]:F2}\t{lastSimulationResult[0]:F2}\tV/V\n" +
                                                            $"Rin:\t\t{TargetValue[1] / 1000:F2}\t{lastSimulationResult[1] / 1000:F2}\tkΩ\n" +
                                                            $"Rout:\t\t{TargetValue[2] / 1000:F2}\t{lastSimulationResult[2] / 1000:F2}\tkΩ\n" +
                                                            $"fL3dB:\t\t{TargetValue[4]:F2}\t{lastSimulationResult[4]:F2}\tHz\n" +
                                                            $"fH3dB:\t\t{TargetValue[3] / 1000:F2}\t{lastSimulationResult[3] / 1000:F2}\tkHz",
            "Wymagania");

        if (timerRunning == false)
        {
            timerRunning = true;
            timerLabel = new Label
            {
                Left = mainForm.Width - 100,
                Top = 10,
                Width = 80,
                Font = new Font("Oxanium", 18, FontStyle.Bold),
                Text = "00:00",
                BackColor = Color.Transparent,
                ForeColor = ColorTranslator.FromHtml("#f7e7af")
            };

            projectTimer = new Timer { Interval = 1000 };
            projectTimer.Tick += (sender, e) =>
            {
                elapsedSeconds++;
                timerLabel.Text = $"{elapsedSeconds / 60:D2}:{elapsedSeconds % 60:D2}";
            };
            projectTimer.Start();
        }

        mainForm.Controls.AddRange(inputRG1, inputRG2, inputRD, inputRS, inputCG, inputCS, inputCD, simulateButton,
            helpButton, notesButton, timerLabel);
    }

    private void Simulate(string inputRG1, string inputRG2, string inputRD, string inputRS, string inputCG,
        string inputCS, string inputCD)
    {
        InputRG1 = inputRG1;
        InputRG2 = inputRG2;
        InputRS = inputRS;
        InputRD = inputRD;
        InputCG = inputCG;
        InputCS = inputCS;
        InputCD = inputCD;

        var culture = CultureInfo.InvariantCulture.Clone() as CultureInfo;
        culture.NumberFormat.NumberDecimalSeparator = ",";

        double.TryParse(inputRG1, NumberStyles.Any, culture, out var RG1);
        double.TryParse(inputRG2, NumberStyles.Any, culture, out var RG2);
        if (SelectedConfiguration == "CD")
        {
            inputRD = "0";
            inputCD = "0";
        }

        double.TryParse(inputRD, NumberStyles.Any, culture, out var RD);
        double.TryParse(inputRS, NumberStyles.Any, culture, out var RS);
        double.TryParse(inputCG, NumberStyles.Any, culture, out var CG);
        double.TryParse(inputCS, NumberStyles.Any, culture, out var CS);
        double.TryParse(inputCD, NumberStyles.Any, culture, out var CD);

        var simulationManager = new SimulationManager(this);
        var results = simulationManager.Simulate(RG1 * 1000, RG2 * 1000, RD * 1000, RS * 1000, CG / 1000000000,
            CS / 1000000000, CD / 1000000000);


        bool isInvalid;
        if (SelectedConfiguration == "CD")
            isInvalid = results[5] <= 0 || results[6] == 0 || RG1 == 0 || RG2 == 0 || RS == 0 || CG == 0 ||
                        CS == 0 || results[7] == 1;
        else
            isInvalid = results[5] <= 0 || results[6] == 0 || RG1 == 0 || RG2 == 0 || RS == 0 || RD == 0 ||
                        CG == 0 || CS == 0 || CD == 0 || results[7] == 1;

        results = results.Take(5).ToArray();
        lastSimulationResult = results;
        simulationManager.ShowSimulationResult(results, isInvalid,
            () => InitializeProjectScreen(),
            () => ShowFinalScreen(results));
    }

    private void ShowFinalScreen(double[] results)
    {
        mainForm.Controls.Clear();
        mainForm.Text = "AmpLab: Sztuka Analogów - Ocena";
        projectTimer.Stop();
        timerRunning = false;

        var differences = new double[results.Length];
        for (var i = 0; i < results.Length; i++)
            differences[i] = Math.Abs(Math.Abs(TargetValue[i] - results[i]) / TargetValue[i]) * 100;

        var dataGridView = new DataGridView
        {
            ColumnCount = 4,
            Columns =
            {
                [0] = { Name = "Wartość" },
                [1] = { Name = "Zadane" },
                [2] = { Name = "Uzyskane" },
                [3] = { Name = "Błąd [%]" }
            },
            Rows =
            {
                {
                    "Wzmocnienie [V/V]", TargetValue[0].ToString("F2"), results[0].ToString("F2"),
                    differences[0].ToString("F2")
                },
                {
                    "Rin [kΩ]", (TargetValue[1] / 1000).ToString("F2"), (results[1] / 1000).ToString("F2"),
                    differences[1].ToString("F2")
                },
                {
                    "Rout [kΩ]", (TargetValue[2] / 1000).ToString("F2"), (results[2] / 1000).ToString("F2"),
                    differences[2].ToString("F2")
                },
                {
                    "fL3dB [Hz]", TargetValue[4].ToString("F2"), results[4].ToString("F2"),
                    differences[4].ToString("F2")
                },
                {
                    "fH3dB [kHz]", (TargetValue[3] / 1000).ToString("F2"), (results[3] / 1000).ToString("F2"),
                    differences[3].ToString("F2")
                }
            },
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            Left = 50,
            Top = 50,
            Width = 600,
            AllowUserToAddRows = false,
            ReadOnly = true
        };

        var sumError = differences.Sum();
        var rating = CalculateRating(sumError, elapsedSeconds, SelectedConfiguration);
        elapsedSeconds = 0;
        InputRG1 = null;
        InputRG2 = null;
        InputRD = null;
        InputRS = null;
        InputCG = null;
        InputCS = null;
        InputCD = null;

        var ratingLabel = new Label
        {
            Text = rating >= 4.5 ? $"Wyśmienicie! Twoja ocena końcowa to: {rating:F1} / 5,0" :
                rating >= 3.0 ? $"Dobra robota! Twoja ocena końcowa to: {rating:F1} / 5,0" :
                rating >= 2.0 ? $"Jesteś na dobrej drodze! Twoja ocena końcowa to: {rating:F1} / 5,0" :
                $"Postaraj sie bardziej! Twoja ocena końcowa to: {rating:F1} / 5,0",
            Left = 50,
            Top = 300,
            AutoSize = true,
            Font = new Font("Arial", 16, FontStyle.Bold)
        };

        var exitButton = new Button
        {
            Text = "Zakończ",
            Left = 50,
            Top = 350,
            Width = 100
        };
        exitButton.Click += (s, e) => InitializeStartScreen();

        mainForm.Controls.AddRange(dataGridView, ratingLabel, exitButton);
    }

    private double CalculateRating(double sumError, int elapsedSeconds, string difficulty)
    {
        double maxTime, maxError, timePenalty, errorPenalty;
        switch (difficulty)
        {
            case "Easy":
                maxTime = 600; // czas 10min
                maxError = 80; // suma błędu 80%
                timePenalty = 120; // kara za każde dodatkowe 2 min
                errorPenalty = 20; // kara za każde dodatkowe 20% błędu
                break;
            case "Medium":
                maxTime = 600; // czas 10 min
                maxError = 60; // suma błędu 60%
                timePenalty = 60; // kara za każdą dodatkową 1 min
                errorPenalty = 10; // kara za każde dodatkowe 10% błędu
                break;
            case "Hard":
                maxTime = 420; // czas 7 min
                maxError = 30; // suma błędu 30%
                timePenalty = 120; // kara za każde dodatkowe 2 min
                errorPenalty = 5; // kara za każde dodatkowe 5% błędu
                break;
            default:
                maxTime = 600;
                maxError = 80;
                timePenalty = 120;
                errorPenalty = 20;
                break;
        }

        var timeFactor = Math.Max(0, elapsedSeconds - maxTime) / timePenalty * 0.5;
        var errorFactor = Math.Max(0, sumError - maxError) / errorPenalty * 0.5;
        return Math.Max(0, Math.Round(5 - timeFactor - errorFactor, 1));
    }
}