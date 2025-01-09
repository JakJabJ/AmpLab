using System;
using System.Windows.Forms;

namespace AmpLab
{
    public class HelpManager
    {
        private string difficulty;
        private string configuration;
        private double RG1;
        private double RG2;
        private double RD;
        private double RS;
        private double CG;
        private double CS;
        private double CD;
        private double[] expectedValues;

        public HelpManager(string difficulty, string configuration, double RG1, double RG2, double RD, double RS, double CG, double CS, double CD, double[] expectedValues)
        {
            this.difficulty = difficulty;
            this.configuration = configuration;
            this.RG1 = RG1;
            this.RG2 = RG2;
            this.RD = RD;
            this.RS = RS;
            this.CG = CG;
            this.CS = CS;
            this.CD = CD;
            this.expectedValues = expectedValues;
        }

        public void ShowHelp()
        {
            string[] elements = { "RG1", "RG2", "RD", "RS", "CG", "CS", "CD" };
            elements = elements.Select(e => e.Replace("\u2093", "G")) // Znak G z kropką dolną jako wizualna alternatywa.
                .ToArray();
            double[] inputs = { RG1, RG2, RD, RS, CG, CS, CD };
            string message = "";

            // Switch based on difficulty
            switch (difficulty)
            {
                case "Easy":
                    message += "Witaj Praktykancie!\n";
                    break;
                case "Medium":
                    message += "Witaj Inżynierze!\n";
                    break;
                case "Hard":
                    message += "Witaj Elektroniku!\n";
                    break;
                default:
                    message += "Wystąpił błąd, nieznany poziom trudności\n";
                    break;
            }

            // Switch based on configuration
            switch (configuration)
            {
                case "CS":
                    message += "Oto wskazówki dotyczące konfiguracji Common Source\n";
                    break;
                case "CG":
                    message += "Oto wskazówki dotyczące konfiguracji Common Gate\n";
                    break;
                case "CD":
                    message += "Oto wskazówki dotyczące konfiguracji Common Drain\n";
                    break;
                default:
                    message += "Wystąpił błąd, nieznana konfiguracja.\n";
                    break;
            }

            switch (difficulty)
            {
                case "Easy":

                    // Compare input values with expected values and provide feedback
                    message += "\nPamiętaj, żeby wykonać symulację po każdych zmianach wartości\n\nPoniżej znajdziesz podpowiedzi, które pomogą Ci obrać właściwy kierunek.\n";
                    for (int i = 0; i < inputs.Length; i++)
                    {
                        
                        double lowerBound = 0.9 * expectedValues[i];
                        double upperBound = 1.1 * expectedValues[i];
                        if (inputs[i] >= lowerBound && inputs[i] <= upperBound)
                        {
                            message += $"Jesteś blisko dla {elements[i]}\n";
                        }
                        else if (inputs[i] < lowerBound)
                        {
                            message += $"Zwiększ {elements[i]}\n";
                        }
                        else
                        {
                            message += $"Zmniejsz {elements[i]}\n";
                        }
                        if(configuration == "CD" && (i==1 || i==5))
                        {
                            i++;
                        }
                    }

                    // Show the message box
                    break;
                case "Medium":
                    switch (configuration)
                    {
                        case "CS":
                            message += "\n1. Wzmocnienie (Ku)\n";
                            message += "   Aby zwiększyć Ku: zmniejsz RS, zwiększ RD.\n";
                            message += "   Aby zmniejszyć Ku: zwiększ RS, zmniejsz RD.\n";
                            message += "\n2. Rezystancja wejściowa (Rin)\n";
                            message += "   Aby zwiększyć Rin: zwiększ RG1 i/lub RG2.\n";
                            message += "   Aby zmniejszyć Rin: zmniejsz RG1 i/lub RG2.\n";
                            message += "\n3. Rezystancja wyjściowa (Rout)\n";
                            message += "   Aby zwiększyć Rout: zwiększ RD.\n";
                            message += "   Aby zmniejszyć Rout: zmniejsz RD.\n";
                            message += "\n4. Częstotliwość dolna 3 dB (fL3dB)\n";
                            message += "   Aby zwiększyć fL3dB: zmniejsz CS, CG i/lub CD.\n";
                            message += "   Aby zmniejszyć fL3dB: zwiększ CS, CG i/lub CD.\n";
                            message += "\n5. Częstotliwość górna 3 dB (fH3dB)\n";
                            message += "   Aby zwiększyć fH3dB: zwiększ RG1, RG2 lub zmniejsz RD.\n";
                            message += "   Aby zmniejszyć fH3dB: zmniejsz RG1, RG2 lub zwiększ RD.\n";
                            break;
                        case "CG":
                            message += "\n1. Wzmocnienie (Ku)\n";
                            message += "   Aby zwiększyć Ku: zmniejsz RS, zwiększ RD.\n";
                            message += "   Aby zmniejszyć Ku: zwiększ RS, zmniejsz RD.\n";
                            message += "\n2. Rezystancja wejściowa (Rin)\n";
                            message += "   Aby zwiększyć Rin: zmniejsz RS, zwiększ gm.\n";
                            message += "   Aby zmniejszyć Rin: zwiększ RS.\n";
                            message += "\n3. Rezystancja wyjściowa (Rout)\n";
                            message += "   Aby zwiększyć Rout: zwiększ RD.\n";
                            message += "   Aby zmniejszyć Rout: zmniejsz RD.\n";
                            message += "\n4. Częstotliwość dolna 3 dB (fL3dB)\n";
                            message += "   Aby zwiększyć fL3dB: zmniejsz CS i/lub CD.\n";
                            message += "   Aby zmniejszyć fL3dB: zwiększ CS i/lub CD.\n";
                            message += "\n5. Częstotliwość górna 3 dB (fH3dB)\n";
                            message += "   Aby zwiększyć fH3dB: zmniejsz RG1, RG2 lub zwiększ RD.\n";
                            message += "   Aby zmniejszyć fH3dB: zwiększ RG1, RG2 lub zmniejsz RD.\n";
                            break;
                        case "CD":
                            message += "\n1. Wzmocnienie (Ku)\n";
                            message += "   Aby zwiększyć Ku: zmniejsz RS, zwiększ RD.\n";
                            message += "   Aby zmniejszyć Ku: zwiększ RS, zmniejsz RD.\n";
                            message += "\n2. Rezystancja wejściowa (Rin)\n";
                            message += "   Aby zwiększyć Rin: zwiększ RG1 i/lub RG2.\n";
                            message += "   Aby zmniejszyć Rin: zmniejsz RG1 i/lub RG2.\n";
                            message += "\n3. Rezystancja wyjściowa (Rout)\n";
                            message += "   Aby zwiększyć Rout: zwiększ RD.\n";
                            message += "   Aby zmniejszyć Rout: zmniejsz RD.\n";
                            message += "\n4. Częstotliwość dolna 3 dB (fL3dB)\n";
                            message += "   Aby zwiększyć fL3dB: zmniejsz CS, CG i/lub CD.\n";
                            message += "   Aby zmniejszyć fL3dB: zwiększ CS, CG i/lub CD.\n";
                            message += "\n5. Częstotliwość górna 3 dB (fH3dB)\n";
                            message += "   Aby zwiększyć fH3dB: zwiększ RG1, RG2 lub zmniejsz RD.\n";
                            message += "   Aby zmniejszyć fH3dB: zmniejsz RG1, RG2 lub zwiększ RD.\n";
                            break;
                        default:
                            message += "Wystąpił błąd.\n";
                            break;
                    }
                    break;
                case "Hard":
                            message += "\n1. Zwiększając wzmocnienie (Ku), obniżasz fH3dB.\n";
                            message += "2. Zmiana rezystorów RG1 i RG2 wpływa na Rin.\n";
                            message += "3. Zmniejszenie CS zwiększa fL3dB, ale obniża Ku.\n";
                            message += "4. Zmiana RD wpływa na Rout oraz Ku.\n";
                            message += "5. Zwiększając RG1 i RG2, poprawiasz fH3dB.\n";
                            message += "6. Zwiększanie RD obniża fH3dB.\n";
                    break;
                default:
                    message += "Wystąpił błąd, nieznany poziom trudności\n";
                    break;
            }
            
            // Show the message box
            // Show the custom message box
            Font font = new Font("Oxanium", 18, FontStyle.Bold);
            Color fontColor = ColorTranslator.FromHtml("#f7e7af");
            string backgroundImagePath = "../../../images/MessageBoxBackground.png";

            CustomMessageBox.Show(message, "Help Information", font, fontColor, backgroundImagePath);
        }
    }
}