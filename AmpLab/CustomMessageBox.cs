using System;
using System.Drawing;
using System.Windows.Forms;

public class CustomMessageBox : Form
{
    private Label messageLabel;
    private Button okButton;

    public CustomMessageBox(string message, string title, Font font, Color fontColor, string backgroundImagePath)
    {
        this.Text = title;
        this.BackgroundImage = Image.FromFile(backgroundImagePath);
        this.BackgroundImageLayout = ImageLayout.Stretch;
        this.FormBorderStyle = FormBorderStyle.FixedDialog; // Prevent resizing
        this.MaximizeBox = false; // Disable maximize button
        this.MinimizeBox = false; // Disable minimize button

        messageLabel = new Label()
        {
            Text = message,
            Font = font,
            ForeColor = fontColor,
            BackColor = Color.Transparent,
            AutoSize = true,
            MaximumSize = new Size(800, 0), // Set a maximum width for the label
            Location = new Point(20, 20)
        };

        okButton = new Button()
        {
            Text = "Zrozumiano!",
            AutoSize = true,
            DialogResult = DialogResult.OK
        };

        this.Controls.Add(messageLabel);
        this.Controls.Add(okButton);
        this.AcceptButton = okButton;

        // Adjust form size based on the label size
        this.Size = new Size(messageLabel.PreferredWidth + 40, messageLabel.PreferredHeight + 100);
        okButton.Location = new Point((this.ClientSize.Width - okButton.Width) / 2, messageLabel.Bottom + 20);

        // Adjust form height to fit the button
        this.Height = okButton.Bottom + 50;
    }

    public static void Show(string message, string title, Font font, Color fontColor, string backgroundImagePath)
    {
        using (var customMessageBox = new CustomMessageBox(message, title, font, fontColor, backgroundImagePath))
        {
            customMessageBox.ShowDialog();
        }
    }
}