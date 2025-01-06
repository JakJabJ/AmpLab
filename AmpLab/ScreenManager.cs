using System;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;

namespace AmpLab
{
    public class ScreenManager
    {
        private System.Windows.Forms.Timer projectTimer;
        private bool timerRunning = false;
        private Label timerLabel;
        private int elapsedSeconds;
        public string InputRG1 { get; set; }
        public string InputRG2 { get; set; }
        public string InputRD { get; set; }
        public string InputRS { get; set; }
        public string InputCG { get; set; }
        public string InputCS { get; set; }
        public string InputCD { get; set; }
        private Form mainForm;
        private Random random = new Random();

        public double[] TargetValue { get; private set; } // Wartość zadana
        public double K = 0, Ri = 0, Ro = 0, fH = 0, fL = 0; // Wartości zadane
        public string SelectedConfiguration { get; private set; } // Wybrana konfiguracja

        private double[] lastSimulationResult = {0,0,0,0,0};

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
                "Easy" => targetEasy(K, Ri, Ro, fH, fL),
                "Medium" => targetMedium(K, Ri, Ro, fH, fL),
                "Hard" => targetHard(K, Ri, Ro, fH, fL),
                _ => new double[] { 0,0,0,0,0 }
            };

            MessageBox.Show($"\tWartości zadane:\n\n" +
                            $"Wzmocnienie:\t{TargetValue[0]:F2} V/V\n" +
                            $"Rin:\t\t{TargetValue[1]/1000:F2} kΩ\n" +
                            $"Rout:\t\t{TargetValue[2]/1000:F2} kΩ\n" +
                            $"fL3dB:\t\t{TargetValue[4]:F2} Hz\n" +
                            $"fH3dB:\t\t{TargetValue[3]/1000:F2} kHz", "Wymagania");
            InitializeConfigurationScreen();
        }

        private double[] targetEasy(double K, double Ri, double Ro, double fH, double fL)
        {
            K = -14;
            Ri = 292000;
            Ro = 47000;
            fH = 172000;
            fL = 80;
            return new double[] { K, Ri, Ro, fH, fL};
        }
        private double[] targetMedium(double K, double Ri, double Ro, double fH, double fL)
        {
            return new double[] { K, Ri, Ro, fH, fL};
        }
        private double[] targetHard(double K, double Ri, double Ro, double fH, double fL)
        {
            return new double[] { K, Ri, Ro, fH, fL};
        }
        // Ekran konfiguracji
        public void InitializeConfigurationScreen()
        {
            mainForm.Controls.Clear();
            mainForm.MaximizeBox = false;
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
            switch (SelectedConfiguration)
            {
                case "CS":
                    mainForm.BackgroundImage = Image.FromFile("../../../images/Background_CS.png");
                    inputRG1.Left = (mainForm.Width / 2) - 155;
                    inputRG1.Top = (mainForm.Height / 2) - 258;
                    inputRG2.Left = (mainForm.Width / 2) - 155;
                    inputRG2.Top = (mainForm.Height / 2) - 43;
                    inputRD.Left = (mainForm.Width / 2) - 28;
                    inputRD.Top = (mainForm.Height / 2) - 258;
                    inputRS.Left = (mainForm.Width / 2) - 28;
                    inputRS.Top = (mainForm.Height / 2) - 43;
                    inputCG.Left = (mainForm.Width / 2) - 268;
                    inputCG.Top = (mainForm.Height / 2) - 120;
                    inputCS.Left = (mainForm.Width / 2) + 142;
                    inputCS.Top = (mainForm.Height / 2) - 43;
                    inputCD.Left = (mainForm.Width / 2) + 45;
                    inputCD.Top = (mainForm.Height / 2) - 180;
                    break;
                case "CD":
                    mainForm.BackgroundImage = Image.FromFile("../../../images/Background_CD.png");
                    break;
                case "CG":
                    mainForm.BackgroundImage = Image.FromFile("../../../images/Background_CG.png");
                    break;
                default:
                    mainForm.BackgroundImage = null;
                    break;
            }
            
            int buttonWidth = 300, buttonHeight = 100, spacing = 20;
            int startX = (mainForm.Width - ((buttonWidth * 3) + (spacing * 2))) / 2;
            int startY = mainForm.Height - buttonHeight - 50;

            var simulateButton = ButtonFactory.CreateCustomButton("../../../images/symulacja_button.png", startX, startY, buttonWidth, buttonHeight);
            var helpButton = ButtonFactory.CreateCustomButton("../../../images/podrecznik_button.png", startX + buttonWidth + spacing, startY, buttonWidth, buttonHeight);
            var notesButton = ButtonFactory.CreateCustomButton("../../../images/notatki_button.png", startX + (buttonWidth + spacing) * 2, startY, buttonWidth, buttonHeight);

            simulateButton.Click += (sender, e) => Simulate(inputRG1.Text, inputRG2.Text, inputRD.Text, inputRS.Text, inputCG.Text, inputCS.Text, inputCD.Text);
            helpButton.Click += (sender, e) => MessageBox.Show("Pomoc: tutaj znajdziesz wskazówki!", "Pomoc");
            notesButton.Click += (sender, e) => MessageBox.Show($"\tWartości zadane\tuzyskane\n\n" +
                                                                $"Wzmocnienie:\t{TargetValue[0]:F2}\t{lastSimulationResult[0]:F2}\tV/V\n" +
                                                                $"Rin:\t\t{TargetValue[1]/1000:F2}\t{lastSimulationResult[1]/1000:F2}\tkΩ\n" +
                                                                $"Rout:\t\t{TargetValue[2]/1000:F2}\t{lastSimulationResult[2]/1000:F2}\tkΩ\n" +
                                                                $"fL3dB:\t\t{TargetValue[4]:F2}\t{lastSimulationResult[4]:F2}\tHz\n" +
                                                                $"fH3dB:\t\t{TargetValue[3]/1000:F2}\t{lastSimulationResult[3]/1000:F2}\tkHz", "Wymagania");

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
                    BackColor = Color.Transparent, // Set background to transparent
                    ForeColor = ColorTranslator.FromHtml("#f7e7af") // Set font color to RGB value
                };
            
                projectTimer = new System.Windows.Forms.Timer { Interval = 1000 };
                projectTimer.Tick += (sender, e) =>
                {
                    elapsedSeconds++;
                    timerLabel.Text = $"{elapsedSeconds / 60:D2}:{elapsedSeconds % 60:D2}";
                };
                projectTimer.Start();
            }
            
            mainForm.Controls.AddRange(new Control[] { inputRG1, inputRG2, inputRD, inputRS, inputCG, inputCS, inputCD, simulateButton, helpButton, notesButton, timerLabel });
        }

        private void Simulate(string inputRG1, string inputRG2, string inputRD, string inputRS, string inputCG, string inputCS, string inputCD)
        {
            InputRG1 = inputRG1;
            InputRG2 = inputRG2;
            InputRD = inputRD;
            InputRS = inputRS;
            InputCG = inputCG;
            InputCS = inputCS;
            InputCD = inputCD;

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
            bool isInvalid;
            if (SelectedConfiguration == "CD")
            {
                isInvalid = results[5] <= 0 || results[6] == 0 || RG1 == 0 || RG2 == 0 || RS == 0 || CG == 0 || CS == 0 || CD == 0 || results[7] == 1;
            }
            else
            {
                isInvalid = results[5] <= 0 || results[6] == 0 || RG1 == 0 || RG2 == 0 || RS == 0 || RD == 0 || CG == 0 || CS == 0 || CD == 0 || results[7] == 1;
            }
            results = results.Take(5).ToArray();
            lastSimulationResult = results;
            simulationManager.ShowSimulationResult(results, isInvalid,
                returnToProject: () => InitializeProjectScreen(),
                submitProject: () => ShowFinalScreen(results));
        }

        private void ShowFinalScreen(double[] results)
        {
            mainForm.Controls.Clear();
            mainForm.Text = "AmpLab: Sztuka Analogów - Ocena";

            double[] differences = new double[results.Length];
            for (int i = 0; i < results.Length; i++)
            {
                differences[i] = Math.Abs(Math.Abs(TargetValue[i] - results[i])/TargetValue[i])*100;
            }

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
                    { "Wzmocnienie [V/V]", TargetValue[0].ToString("F2"), results[0].ToString("F2"), differences[0].ToString("F2") },
                    { "Rin [kΩ]", (TargetValue[1]/1000).ToString("F2"), (results[1]/1000).ToString("F2"), differences[1].ToString("F2") },
                    { "Rout [kΩ]", (TargetValue[2]/1000).ToString("F2"), (results[2]/1000).ToString("F2"), differences[2].ToString("F2") },
                    { "fL3dB [Hz]", TargetValue[4].ToString("F2"), results[4].ToString("F2"), differences[4].ToString("F2") },
                    { "fH3dB [kHz]", (TargetValue[3]/1000).ToString("F2"), (results[3]/1000).ToString("F2"), differences[3].ToString("F2") }
                },
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Left = 50,
                Top = 50,
                Width = 600,
                AllowUserToAddRows = false,
                ReadOnly = true
                //Height = 200
            };

            var exitButton = new Button
            {
                Text = "Zakończ",
                Left = 50,
                Top = 300,
                Width = 100
            };
            exitButton.Click += (s, e) => InitializeStartScreen();

            mainForm.Controls.AddRange(new Control[] { dataGridView, exitButton });
        }
    }
}