namespace AmpLab;

public class SimulationManager
{
    private readonly ScreenManager screenManager;

    public SimulationManager(ScreenManager manager)
    {
        screenManager = manager;
    }

    public double[] Simulate(double RG1, double RG2, double RD, double RS, double CG, double CS, double CD)
    {
        return screenManager.SelectedConfiguration switch
        {
            "CS" => SimulateCS(RG1, RG2, RD, RS, CG, CS, CD),
            "CG" => SimulateCG(RG1, RG2, RD, RS, CG, CS, CD),
            "CD" => SimulateCD(RG1, RG2, RD, RS, CG, CS, CD),
            _ => new double[] { 0 }
        };
    }

    public double[] SimulateCS(double RG1, double RG2, double RD, double RS, double CG, double CS, double CD)
    {
        double error = 1;
        // Zmienne do obliczeń
        double Id = 0, delta = -1;
        double Vgs = 0;
        double Vs = 0;
        double Vd = 0;
        double Vds = 0;
        double x1 = 0, x2 = 0, gm = 0;
        var isInvalid = false;

        // Dane
        var Kn = 312.32e-6;
        var Vt = 1.272;
        double Vdd = 12;
        var RL = double.PositiveInfinity;
        var Rbuf = 10e6;
        double Rgen = 4600;
        var Cgd = 1.7e-12;
        var Cgs = 9.33e-12;
        var Cds = 12.34e-12;
        var Cbuf = 3e-12;

        // punkt pracy
        var Vg = Vdd * (RG2 / (RG1 + RG2));
        var a = Kn * Math.Pow(RS, 2);
        var b = Kn * RS * (Vt - 2 * Vg) - 1;
        var c = Kn * Math.Pow(Vg - Vt, 2);
        delta = Math.Pow(b, 2) - 4 * a * c;

        if (delta > 0)
        {
            x1 = (-b + Math.Sqrt(delta)) / (2 * a);
            x2 = (-b - Math.Sqrt(delta)) / (2 * a);

            foreach (var x in new[] { x1, x2 })
                if (Vg - x * RS > Vt)
                {
                    Id = x;
                    Vgs = Vg - Id * RS;
                    Vs = Id * RS;
                    Vd = Vdd - Id * RD;
                    Vds = Vd - Vs;

                    if (Vgs - Vt <= Vds)
                    {
                        gm = 2 * Math.Sqrt(Kn * Id);
                        error = 0;
                        break;
                    }
                }
        }
        else if (delta == 0)
        {
            x1 = -b / (2 * a);
            if (Vg - x1 * RS > Vt)
            {
                Id = x1;
                Vgs = Vg - Id * RS;
                Vs = Id * RS;
                Vd = Vdd - Id * RD;
                Vds = Vd - Vs;

                if (Vgs - Vt <= Vds)
                {
                    gm = 2 * Math.Sqrt(Kn * Id);
                    error = 0;
                }
            }
        }

        // analiza małosygnałowa
        var Rin = 1 / (1 / RG1 + 1 / RG2);
        var Rout = RD;
        var fKuA = -(Rin / (Rin + Rgen)) * (gm * (1 / (1 / RD + 1 / RL + 1 / Rbuf)));
        var Ku = fKuA;
        var fKA = -gm * (1 / (1 / RD + 1 / RL + 1 / Rbuf));
        var K = fKA;
        var Cm1 = Cgd * (1 - K);
        var Cm2 = Cgd * (1 - 1 / K);
        var Th1 = (Cm1 + Cgs) * (1 / (1 / Rin + 1 / Rgen));
        var fTh2A = (Cm2 + Cbuf + Cds) * (1 / (1 / RD + 1 / RL + 1 / Rbuf));
        var Th2 = fTh2A;
        var fH3dB = 1 / (2 * Math.PI * (Th1 + Th2));
        var Tl1 = CG * (Rgen + 1 / (1 / RG1 + 1 / RG2));
        var Tl2 = CS * (1 / (1 / Rout + gm));
        var fTl3A = CD * (RD + 1 / (1 / RL + 1 / Rbuf));
        var Tl3 = fTl3A;
        var fL3dB = 1 / (2 * Math.PI) * Math.Sqrt(1 / Math.Pow(Tl1, 2) + 1 / Math.Pow(Tl2, 2) + 1 / Math.Pow(Tl3, 2));
        return new[] { Ku, Rin, Rout, fH3dB, fL3dB, delta, Id, error };
    }

    public double[] SimulateCG(double RG1, double RG2, double RD, double RS, double CG, double CS, double CD)
    {
        double error = 1;
        // zmienna do obliczeń
        double Id = 0, delta = -1;
        double Vgs = 0;
        double Vs = 0;
        double Vd = 0;
        double Vds = 0;
        double x1 = 0, x2 = 0, gm = 0;
        var isInvalid = false;

        // Dane
        var Kn = 323.46e-6;
        var Vt = 1.264;
        double Vdd = 12;
        var RL = double.PositiveInfinity;
        var Rbuf = 10e6;
        double Rgen = 4600;
        var Cgd = 1.84e-12;
        var Cgs = 9.74e-12;
        var Cds = 12.36e-12;
        var Cbuf = 3e-12;

        // punkt pracy
        var Vg = Vdd * (RG2 / (RG1 + RG2));
        var a = Kn * Math.Pow(RS, 2);
        var b = Kn * RS * (Vt - 2 * Vg) - 1;
        var c = Kn * Math.Pow(Vg - Vt, 2);
        delta = Math.Pow(b, 2) - 4 * a * c;

        if (delta > 0)
        {
            x1 = (-b + Math.Sqrt(delta)) / (2 * a);
            x2 = (-b - Math.Sqrt(delta)) / (2 * a);

            foreach (var x in new[] { x1, x2 })
                if (Vg - x * RS > Vt)
                {
                    Id = x;
                    Vgs = Vg - Id * RS;
                    Vs = Id * RS;
                    Vd = Vdd - Id * RD;
                    Vds = Vd - Vs;

                    if (Vgs - Vt <= Vds)
                    {
                        gm = 2 * Math.Sqrt(Kn * Id);
                        error = 0;
                        break;
                    }
                }
        }
        else if (delta == 0)
        {
            x1 = -b / (2 * a);
            if (Vg - x1 * RS > Vt)
            {
                Id = x1;
                Vgs = Vg - Id * RS;
                Vs = Id * RS;
                Vd = Vdd - Id * RD;
                Vds = Vd - Vs;

                if (Vgs - Vt <= Vds)
                {
                    gm = 2 * Math.Sqrt(Kn * Id);
                    error = 0;
                }
            }
        }

        // analiza małosygnałowa
        var Rin = 1 / (1 / RS + gm);
        var Rout = RD;
        Func<double, double> fKuA = RL => Rin / (Rin + Rgen) * (gm * (1 / (1 / RD + 1 / RL + 1 / Rbuf)));
        var Ku = fKuA(double.PositiveInfinity);
        var Th1 = Cgs * (1 / (1 / Rgen + 1 / RS + gm));
        Func<double, double> fTh2B = RL => (Cgd + Cbuf) * (1 / (1 / RD + 1 / RL + 1 / Rbuf));
        var Th2 = fTh2B(double.PositiveInfinity);
        Func<double, double> fTh3B = RL =>
            Cds * ((1 / (1 / Rgen + 1 / RS) + 1 / (1 / RD + 1 / RL + 1 / Rbuf)) / (1 + gm * (1 / (1 / Rgen + RS))));
        var Th3 = fTh3B(double.PositiveInfinity);
        var fH3dB = 1 / (2 * Math.PI * Math.Sqrt(Math.Pow(Th1, 2) + Math.Pow(Th2, 2) + Math.Pow(Th3, 2)));
        var Tl1 = CS * (Rgen + 1 / (1 / RS + gm));
        Func<double, double> fTl2B = RL => CD * (RD + 1 / (1 / RL + 1 / Rbuf));
        var Tl2 = fTl2B(double.PositiveInfinity);
        var fL3dB = 1 / (2 * Math.PI) * Math.Sqrt(1 / Math.Pow(Tl1, 2) + 1 / Math.Pow(Tl2, 2));
        return new[] { Ku, Rin, Rout, fH3dB, fL3dB, delta, Id, error };
    }

    public double[] SimulateCD(double RG1, double RG2, double RD, double RS, double CG, double CS, double CD)
    {
        RD = 0;
        CD = 0;
        double error = 1;
        // zmienne do obliczeń
        double Id = 0, delta = -1;
        double Vgs = 0;
        double Vs = 0;
        double Vd = 0;
        double Vds = 0;
        double x1 = 0, x2 = 0, gm = 0;
        var isInvalid = false;

        // Dane
        var C0C = 35e-12;
        var Kn = 309.15e-6;
        var Vt = 1.252;
        double Vdd = 12;
        var RL = 13.35;
        var Rbuf = 10e6;
        double Rgen = 4600;
        var Cgd = 1.93e-12;
        var Cgs = 8.99e-12;
        var Cds = 11.42e-12;
        var Cbuf = 3e-12;

        // punkt pracy
        var Vg = Vdd * (RG2 / (RG1 + RG2));
        var a = Kn * Math.Pow(RS, 2);
        var b = Kn * RS * (Vt - 2 * Vg) - 1;
        var c = Kn * Math.Pow(Vg - Vt, 2);
        delta = Math.Pow(b, 2) - 4 * a * c;

        if (delta > 0)
        {
            x1 = (-b + Math.Sqrt(delta)) / (2 * a);
            x2 = (-b - Math.Sqrt(delta)) / (2 * a);

            foreach (var x in new[] { x1, x2 })
                if (Vg - x * RS > Vt)
                {
                    Id = x;
                    Vgs = Vg - Id * RS;
                    Vs = Id * RS;
                    Vd = Vdd - Id * RD;
                    Vds = Vd - Vs;

                    if (Vgs - Vt <= Vds)
                    {
                        gm = 2 * Math.Sqrt(Kn * Id);
                        error = 0;
                        break;
                    }
                }
        }
        else if (delta == 0)
        {
            x1 = -b / (2 * a);
            if (Vg - x1 * RS > Vt)
            {
                Id = x1;
                Vgs = Vg - Id * RS;
                Vs = Id * RS;
                Vd = Vdd - Id * RD;
                Vds = Vd - Vs;

                if (Vgs - Vt <= Vds)
                {
                    gm = 2 * Math.Sqrt(Kn * Id);
                    error = 0;
                }
            }
        }

        //analiza małosygnałowa
        var Rin = 1 / (1 / RG1 + 1 / RG2);
        var Rout = 1 / (1 / RS + gm);
        var Ku = Rin / (Rin + Rgen) *
                 (gm * (1 / (1 / RS + 1 / RL + 1 / Rbuf)) / (1 + gm * (1 / (1 / RS + 1 / RL + 1 / Rbuf))));
        var Th1 = Cgd * (1 / (1 / Rgen + 1 / RG1 + 1 / RG2));
        var Th2 = (Cds + C0C + Cbuf) * (1 / (gm + 1 / RS + 1 / RL + 1 / Rbuf));
        var Th3 = Cgs * ((1 / (1 / Rgen + 1 / RG1 + RG2) + 1 / (1 / RS + 1 / RL + 1 / Rbuf)) /
                         (1 + gm * (1 / (1 / RS + 1 / RL + 1 / Rbuf))));
        var fH3dB = 1 / (2 * Math.PI * Math.Sqrt(Math.Pow(Th1, 2) + Math.Pow(Th2, 2) + Math.Pow(Th3, 2)));
        var Tl1 = CG * (Rgen + 1 / (1 / RG1 + 1 / RG2));
        var Tl2 = CS * (1 / (gm + 1 / RS) + 1 / (1 / RL + 1 / Rbuf));
        var fL3dB = 1 / (2 * Math.PI) * Math.Sqrt(1 / Math.Pow(Tl1, 2) + 1 / Math.Pow(Tl2, 2));
        return new[] { Ku, Rin, Rout, fH3dB, fL3dB, delta, Id, error };
    }

    public void ShowSimulationResult(double[] results, bool isInvalid, Action returnToProject, Action submitProject)
    {
        var simulationResultForm = new Form
        {
            Text = "Wynik Symulacji",
            Width = 700,
            Height = 400,
            StartPosition = FormStartPosition.CenterParent,

            BackgroundImage = Image.FromFile("../../../images/MessageBoxBackground.png"),
            BackgroundImageLayout = ImageLayout.Stretch
        };

        if (isInvalid)
        {
            var errorLabel = new Label
            {
                Text =
                    "Tranzystor nie pracuje w zakresie nasycenia! Wzmacniacz nie działa, zwróć szczególną uwagę na rezystory",
                Left = 20,
                Top = 20,
                AutoSize = true
            };
            simulationResultForm.Controls.Add(errorLabel);
        }
        else
        {
            var differences = new double[results.Length];

            var dataGridView = new DataGridView
            {
                ColumnCount = 3,
                Columns =
                {
                    [0] = { Name = "Wartość" },
                    [1] = { Name = "Zadane" },
                    [2] = { Name = "Uzyskane" }
                },
                Rows =
                {
                    { "Wzmocnienie [V/V]", screenManager.TargetValue[0].ToString("F2"), results[0].ToString("F2") },
                    {
                        "Rin [kΩ]", (screenManager.TargetValue[1] / 1000).ToString("F2"),
                        (results[1] / 1000).ToString("F2")
                    },
                    {
                        "Rout [kΩ]", (screenManager.TargetValue[2] / 1000).ToString("F2"),
                        (results[2] / 1000).ToString("F2")
                    },
                    { "fL3dB [Hz]", screenManager.TargetValue[4].ToString("F2"), results[4].ToString("F2") },
                    {
                        "fH3dB [kHz]", (screenManager.TargetValue[3] / 1000).ToString("F2"),
                        (results[3] / 1000).ToString("F2")
                    }
                },
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Left = 50,
                Top = 50,
                Width = 600,
                AllowUserToAddRows = false,
                ReadOnly = true
            };

            simulationResultForm.Controls.Add(dataGridView);
        }

        var returnButton = new Button
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
            var submitButton = new Button
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