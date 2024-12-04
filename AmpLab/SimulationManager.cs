using System;

namespace AmpLab
{
    public class SimulationManager
    {
        private ScreenManager screenManager;

        public SimulationManager(ScreenManager manager)
        {
            screenManager = manager;
        }

        public double Simulate(double value1, double value2)
        {
            return screenManager.SelectedConfiguration switch
            {
                "CS" => value1 + value2,
                "CG" => value1 * value2,
                "CD" => value1 - value2,
                _ => 0
            };
        }
        
        public void ShowSimulationResult(double result, bool isInvalid, Action returnToProject, Action submitProject)
        {
            Form simulationResultForm = new Form
            {
                Text = "Wynik Symulacji",
                Width = 300,
                Height = 200,
                StartPosition = FormStartPosition.CenterParent,
                BackgroundImage = Image.FromFile("../../../images/Background.png"),
                BackgroundImageLayout = ImageLayout.Stretch
            };

            Label resultLabel = new Label
            {
                Text = isInvalid ? "Wynik nie może być mniejszy od 0" : $"Wynik: {result:F2}",
                Left = 20,
                Top = 20,
                AutoSize = true
            };

            Button returnButton = new Button
            {
                Text = "Wróć do Projektowania",
                Left = 160,
                Top = 60,
                Width = 120
            };
            returnButton.Click += (s, args) =>
            {
                simulationResultForm.Close();
                returnToProject?.Invoke();
            };

            simulationResultForm.Controls.Add(resultLabel);
            simulationResultForm.Controls.Add(returnButton);

            if (!isInvalid)
            {
                Button submitButton = new Button
                {
                    Text = "Oddaj Projekt",
                    Left = 20,
                    Top = 60,
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