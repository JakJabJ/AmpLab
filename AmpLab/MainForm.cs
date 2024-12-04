using System;
using System.Drawing;
using System.Windows.Forms;

namespace AmpLab
{
    public class MainForm : Form
    {
        private ScreenManager screenManager;
        private SimulationManager simulationManager;

        public MainForm()
        {
            Width = 1240;
            Height = 720;
            MinimumSize = new Size(1240, 720);
            MaximumSize = new Size(1240, 720);

            // Enable double buffering
            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();

            screenManager = new ScreenManager(this);
            simulationManager = new SimulationManager(screenManager);

            screenManager.InitializeStartScreen();
        }

        [STAThread]
        static void Main()
        {
            Application.Run(new MainForm());
        }
    }
}