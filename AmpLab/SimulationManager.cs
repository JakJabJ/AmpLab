namespace AmpLab
{
    public class SimulationManager
    {
        private ScreenManager screenManager;

        public SimulationManager(ScreenManager manager)
        {
            screenManager = manager;
        }

        public double[] Simulate(double RG1, double RG2, double RD, double RS, double CG, double CS, double CD)
        {
            return screenManager.SelectedConfiguration switch
            {
                "CS" => SimulateCS(RG1, RG2, RD, RS, CG, CS, CD),
                "CG" => new double[] { 2, 2, 2, 2, 2 }, // Placeholder for CG configuration
                "CD" => new double[] { 2 },             // Placeholder for CD configuration
                _ => new double[] { 0 }
            };
        }

        private double[] SimulateCS(double RG1, double RG2, double RD, double RS, double CG, double CS, double CD)
        {
            double error = 1;
            // Initialize variables
            double Id = 0, delta = -1;
            double Vgs = 0;
            double Vs = 0;
            double Vd = 0;
            double Vds = 0;
            double x1 = 0, x2 = 0, gm = 0;
            bool isInvalid = false;

            // Data
            double Kn = 312.32e-6;
            double Vt = 1.272;
            double Vdd = 12;
            double RL = double.PositiveInfinity;
            double Rbuf = 10e6;
            double Rgen = 4600;
            double Cgd = 1.7e-12;
            double Cgs = 9.33e-12;
            double Cds = 12.34e-12;
            double Cbuf = 3e-12;

            // Operating point
            double Vg = Vdd * (RG2 / (RG1 + RG2));
            double a = Kn * Math.Pow(RS, 2);
            double b = (Kn * RS * (Vt - 2 * Vg) - 1);
            double c = Kn * Math.Pow((Vg - Vt), 2);
            delta = Math.Pow(b, 2) - 4 * a * c;

            if (delta > 0)
            {
                x1 = (-b + Math.Sqrt(delta)) / (2 * a);
                x2 = (-b - Math.Sqrt(delta)) / (2 * a);

                foreach (var x in new[] { x1, x2 })
                {
                    if ((Vg - x * RS) > Vt)
                    {
                        Id = x;
                        Vgs = Vg - Id * RS;
                        Vs = Id * RS;
                        Vd = Vdd - Id * RD;
                        Vds = Vd - Vs;

                        if ((Vgs - Vt) <= Vds)
                        {
                            gm = 2 * Math.Sqrt(Kn * Id);
                            error = 0;
                            break;
                        }
                    }
                }
            }
            else if (delta == 0)
            {
                x1 = -b / (2 * a);
                if ((Vg - x1 * RS) > Vt)
                {
                    Id = x1;
                    Vgs = Vg - Id * RS;
                    Vs = Id * RS;
                    Vd = Vdd - Id * RD;
                    Vds = Vd - Vs;

                    if ((Vgs - Vt) <= Vds)
                    {
                        gm = 2 * Math.Sqrt(Kn * Id);
                        error = 0;
                        
                    }
                }
            }
            else
            {
            }

            // Small-signal analysis
            double Rin = 1 / ((1 / RG1) + (1 / RG2));
            double Rout = RD;

            // Calculate Ku
            double fKuA = -((Rin) / (Rin + Rgen)) * (gm * (1 / ((1 / RD) + (1 / RL) + (1 / Rbuf))));
            double Ku = fKuA;

            // Calculate K
            double fKA = -gm * (1 / ((1 / RD) + (1 / RL) + (1 / Rbuf)));
            double K = fKA;

            // Calculate Cm1 and Cm2
            double Cm1 = Cgd * (1 - K);
            double Cm2 = Cgd * (1 - (1 / K));

            // Calculate Th1
            double Th1 = (Cm1 + Cgs) * (1 / ((1 / Rin) + (1 / Rgen)));

            // Calculate Th2
            double fTh2A = (Cm2 + Cbuf + Cds) * (1 / ((1 / RD) + (1 / RL) + (1 / Rbuf)));
            double Th2 = fTh2A;

            // Calculate fH3dB
            double fH3dB = 1 / (2 * Math.PI * (Th1 + Th2));

            // Calculate Tl1 and Tl2
            double Tl1 = CG * (Rgen + (1 / ((1 / RG1) + (1 / RG2))));
            double Tl2 = CS * (1 / ((1 / Rout) + gm));

            // Calculate Tl3
            double fTl3A = CD * (RD + (1 / ((1 / RL) + (1 / Rbuf))));
            double Tl3 = fTl3A;

            // Calculate fL3dB
            double fL3dB = (1 / (2 * Math.PI)) * Math.Sqrt((1 / Math.Pow(Tl1, 2)) + (1 / Math.Pow(Tl2, 2)) + (1 / Math.Pow(Tl3, 2)));

            //Console.WriteLine($"Vg: {Vg}, a: {a}, b: {b}, c: {c}, delta: {delta}");

            return new double[] { Ku, Rin, Rout, fH3dB, fL3dB, delta, Id, error};
        }

        public void ShowSimulationResult(double[] results, bool isInvalid, Action returnToProject, Action submitProject)
        {
            Form simulationResultForm = new Form
            {
                Text = "Wynik Symulacji",
                Width = 700,
                Height = 400,
                StartPosition = FormStartPosition.CenterParent,
                BackgroundImage = Image.FromFile("../../../images/Background.png"),
                BackgroundImageLayout = ImageLayout.Stretch
            };

            if (isInvalid)
            {
                Label errorLabel = new Label
                {
                    Text = "Tranzystor nie pracuje w zakresie nasycenia! Wzmacniacz nie działa, zwróć szczególną uwagę na rezystory",
                    Left = 20,
                    Top = 20,
                    AutoSize = true
                };
                simulationResultForm.Controls.Add(errorLabel);
            }
            else
            {
                double[] differences = new double[results.Length];

                var dataGridView = new DataGridView
                {
                    ColumnCount = 3,
                    Columns =
                    {
                        [0] = { Name = "Wartość" },
                        [1] = { Name = "Zadane" },
                        [2] = { Name = "Uzyskane" },
                    },
                    Rows =
                    {
                        { "Wzmocnienie [V/V]", screenManager.TargetValue[0].ToString("F2"), results[0].ToString("F2")},
                        { "Rin [kΩ]", (screenManager.TargetValue[1] / 1000).ToString("F2"), (results[1] / 1000).ToString("F2")},
                        { "Rout [kΩ]", (screenManager.TargetValue[2] / 1000).ToString("F2"), (results[2] / 1000).ToString("F2")},
                        { "fL3dB [Hz]", screenManager.TargetValue[4].ToString("F2"), results[4].ToString("F2")},
                        { "fH3dB [kHz]", (screenManager.TargetValue[3] / 1000).ToString("F2"), (results[3] / 1000).ToString("F2")}
                    },
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    Left = 50,
                    Top = 50,
                    Width = 600,
                    //Height = 200,
                    AllowUserToAddRows = false,
                    ReadOnly = true
                };

                simulationResultForm.Controls.Add(dataGridView);
            }

            Button returnButton = new Button
            {
                Text = "Wróć do Projektowania",
                Left = 160,
                Top = 300,
                Width = 120
            };
            returnButton.Click += (s, args) =>
            {
                simulationResultForm.Close();
                returnToProject?.Invoke();
            };

            simulationResultForm.Controls.Add(returnButton);

            if (!isInvalid)
            {
                Button submitButton = new Button
                {
                    Text = "Oddaj Projekt",
                    Left = 20,
                    Top = 300,
                    Width = 120
                };
                submitButton.Click += (s, args) =>
                {
                    simulationResultForm.Close();
                    submitProject?.Invoke();
                };

                simulationResultForm.Controls.Add(submitButton);
            }

            simulationResultForm.ShowDialog();
        }
    }
}