using System.Drawing.Drawing2D;

namespace AmpLab;

public static class ButtonFactory
{
    public static Button CreateCustomButton(string imagePath, int left, int top, int width, int height)
    {
        var button = new Button
        {
            Left = left,
            Top = top,
            Width = width,
            Height = height,
            BackgroundImage = Image.FromFile(imagePath),
            BackgroundImageLayout = ImageLayout.Stretch,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.Transparent,
            FlatAppearance = { BorderSize = 0, MouseOverBackColor = Color.Transparent }
        };

        button.Paint += (sender, e) =>
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var radius = 90;
            var shadowOffset = 3;

            // Cień na przycisku
            var shadowPath = new GraphicsPath();
            shadowPath.AddArc(shadowOffset, shadowOffset, radius, radius, 180, 90);
            shadowPath.AddArc(width - radius + shadowOffset, shadowOffset, radius, radius, 270, 90);
            shadowPath.AddArc(width - radius + shadowOffset, height - radius + shadowOffset, radius, radius, 0, 90);
            shadowPath.AddArc(shadowOffset, height - radius + shadowOffset, radius, radius, 90, 90);
            shadowPath.CloseFigure();

            using (var brush = new PathGradientBrush(shadowPath))
            {
                brush.CenterColor = Color.FromArgb(50, Color.Black);
                brush.SurroundColors = new[] { Color.Transparent };
                g.FillPath(brush, shadowPath);
            }

            // zaokrąglone krawędzie przycisku
            var buttonPath = new GraphicsPath();
            buttonPath.AddArc(0, 0, radius, radius, 180, 90);
            buttonPath.AddArc(width - radius, 0, radius, radius, 270, 90);
            buttonPath.AddArc(width - radius, height - radius, radius, radius, 0, 90);
            buttonPath.AddArc(0, height - radius, radius, radius, 90, 90);
            buttonPath.CloseFigure();

            button.Region = new Region(buttonPath);

            // hover
            if (button.ClientRectangle.Contains(button.PointToClient(Cursor.Position)))
                using (var hoverPen = new Pen(Color.FromArgb(80, Color.Black), 25))
                {
                    g.DrawPath(hoverPen, buttonPath);
                }
        };

        return button;
    }
}