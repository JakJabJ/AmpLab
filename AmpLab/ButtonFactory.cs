using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace AmpLab
{
    public static class ButtonFactory
    {
        public static Button CreateCustomButton(string imagePath, int left, int top, int width, int height)
        {
            Button button = new Button
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
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                int radius = 90;
                int shadowOffset = 3;

                // Draw shadow
                GraphicsPath shadowPath = new GraphicsPath();
                shadowPath.AddArc(shadowOffset, shadowOffset, radius, radius, 180, 90);
                shadowPath.AddArc(width - radius + shadowOffset, shadowOffset, radius, radius, 270, 90);
                shadowPath.AddArc(width - radius + shadowOffset, height - radius + shadowOffset, radius, radius, 0, 90);
                shadowPath.AddArc(shadowOffset, height - radius + shadowOffset, radius, radius, 90, 90);
                shadowPath.CloseFigure();

                using (PathGradientBrush brush = new PathGradientBrush(shadowPath))
                {
                    brush.CenterColor = Color.FromArgb(50, Color.Black);
                    brush.SurroundColors = new Color[] { Color.Transparent };
                    g.FillPath(brush, shadowPath);
                }

                // Draw button with rounded corners
                GraphicsPath buttonPath = new GraphicsPath();
                buttonPath.AddArc(0, 0, radius, radius, 180, 90);
                buttonPath.AddArc(width - radius, 0, radius, radius, 270, 90);
                buttonPath.AddArc(width - radius, height - radius, radius, radius, 0, 90);
                buttonPath.AddArc(0, height - radius, radius, radius, 90, 90);
                buttonPath.CloseFigure();

                button.Region = new Region(buttonPath);

                // Draw hover effect
                if (button.ClientRectangle.Contains(button.PointToClient(Cursor.Position)))
                {
                    using (Pen hoverPen = new Pen(Color.FromArgb(80, Color.Black), 25))
                    {
                        g.DrawPath(hoverPen, buttonPath);
                    }
                }
            };

            return button;
        }
    }
}