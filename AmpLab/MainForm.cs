namespace AmpLab;

public class MainForm : Form
{
    private readonly ScreenManager screenManager;
    private SimulationManager simulationManager;

    public MainForm()
    {
        Width = 1240;
        Height = 720;
        MinimumSize = new Size(1240, 720);
        MaximumSize = new Size(1240, 720);

        // Naprawienie migającego ekranu
        SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
        UpdateStyles();

        screenManager = new ScreenManager(this);
        simulationManager = new SimulationManager(screenManager);

        screenManager.InitializeStartScreen();
    }

    [STAThread]
    private static void Main()
    {
        Application.Run(new MainForm());
    }
}