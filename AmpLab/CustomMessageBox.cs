public class CustomMessageBox : Form
{
    private readonly Label messageLabel;
    private readonly Button okButton;

    public CustomMessageBox(string message, string title, Font font, Color fontColor, string backgroundImagePath)
    {
        Text = title;
        BackgroundImage = Image.FromFile(backgroundImagePath);
        BackgroundImageLayout = ImageLayout.Stretch;
        FormBorderStyle = FormBorderStyle.FixedDialog; // blokada zmiany rozmiaru okna
        MaximizeBox = false; // blokada maksymalizacji okna
        MinimizeBox = false; // blokada minimalizacji okna

        messageLabel = new Label
        {
            Text = message,
            Font = font,
            ForeColor = fontColor,
            BackColor = Color.Transparent,
            AutoSize = true,
            MaximumSize = new Size(800, 0), // maksymalna szerokość okna
            Location = new Point(20, 20)
        };

        okButton = new Button
        {
            Text = "Zrozumiano!",
            AutoSize = true,
            DialogResult = DialogResult.OK
        };

        Controls.Add(messageLabel);
        Controls.Add(okButton);
        AcceptButton = okButton;

        // dopasowanie rozmiaru okna do zawartości
        Size = new Size(messageLabel.PreferredWidth + 40, messageLabel.PreferredHeight + 100);
        okButton.Location = new Point((ClientSize.Width - okButton.Width) / 2, messageLabel.Bottom + 20);

        // dopasowanie rozmiaru okna do zawartości
        Height = okButton.Bottom + 50;
    }

    public static void Show(string message, string title, Font font, Color fontColor, string backgroundImagePath)
    {
        using (var customMessageBox = new CustomMessageBox(message, title, font, fontColor, backgroundImagePath))
        {
            customMessageBox.ShowDialog();
        }
    }
}