using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace VMTLauncher.Controls
{
    /// <summary>
    /// Custom-painted modern button with rounded corners, gradient fill, and hover effects.
    /// </summary>
    public class ThemedButton : Button
    {
        private bool _isHovered = false;
        private bool _isPressed = false;
        private Color _baseColor = ThemeColors.AccentBlue;
        private Color _hoverColor = ThemeColors.AccentBlueHover;
        private int _cornerRadius = 8;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color BaseColor
        {
            get => _baseColor;
            set { _baseColor = value; Invalidate(); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color HoverColor
        {
            get => _hoverColor;
            set { _hoverColor = value; Invalidate(); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int CornerRadius
        {
            get => _cornerRadius;
            set { _cornerRadius = value; Invalidate(); }
        }

        public ThemedButton()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            Font = ThemeColors.FontButton;
            ForeColor = ThemeColors.TextHighlight;
            Cursor = Cursors.Hand;
            Height = 40;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            _isHovered = true;
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _isHovered = false;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            _isPressed = true;
            Invalidate();
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _isPressed = false;
            Invalidate();
            base.OnMouseUp(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Clear the entire control background first to prevent ghost artifacts
            g.Clear(Parent?.BackColor ?? ThemeColors.BackgroundDark);

            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            using var path = CreateRoundedRect(rect, _cornerRadius);

            // Determine fill color — use fully opaque colors only
            Color fillColor;
            if (!Enabled)
            {
                // Solid dark muted color — no alpha blending
                fillColor = Color.FromArgb(30, 38, 52);
            }
            else if (_isPressed)
                fillColor = ControlPaint.Dark(_hoverColor, 0.1f);
            else if (_isHovered)
                fillColor = _hoverColor;
            else
                fillColor = _baseColor;

            // Draw fill (solid brush for disabled, gradient for enabled)
            if (!Enabled)
            {
                using var brush = new SolidBrush(fillColor);
                g.FillPath(brush, path);

                // Subtle border for disabled state
                using var borderPen = new Pen(Color.FromArgb(40, 50, 68), 1f);
                g.DrawPath(borderPen, path);
            }
            else
            {
                // Draw subtle gradient fill
                using var brush = new LinearGradientBrush(rect,
                    Color.FromArgb(Math.Min(255, fillColor.R + 15),
                                   Math.Min(255, fillColor.G + 15),
                                   Math.Min(255, fillColor.B + 15)),
                    fillColor,
                    LinearGradientMode.Vertical);

                g.FillPath(brush, path);
            }

            // Draw glow effect on hover
            if (_isHovered && Enabled)
            {
                using var glowPen = new Pen(Color.FromArgb(60, 255, 255, 255), 1f);
                g.DrawPath(glowPen, path);
            }

            // Draw text — fully opaque muted for disabled
            var textColor = Enabled ? ForeColor : Color.FromArgb(70, 80, 100);
            TextRenderer.DrawText(g, Text, Font, rect, textColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        private static GraphicsPath CreateRoundedRect(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int diameter = radius * 2;
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }
    }

    /// <summary>
    /// Custom-painted progress bar with rounded track, gradient fill, and status text.
    /// </summary>
    public class ThemedProgressBar : Control
    {
        private int _value = 0;
        private int _maximum = 100;
        private string _statusText = "";

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Value
        {
            get => _value;
            set { _value = Math.Clamp(value, 0, _maximum); Invalidate(); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Maximum
        {
            get => _maximum;
            set { _maximum = Math.Max(1, value); Invalidate(); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string StatusText
        {
            get => _statusText;
            set { _statusText = value; Invalidate(); }
        }

        public ThemedProgressBar()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            Height = 28;
            BackColor = ThemeColors.ProgressTrack;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            int radius = Height / 2;

            // Track
            using (var trackPath = CreateRoundedRect(rect, radius))
            using (var trackBrush = new SolidBrush(ThemeColors.ProgressTrack))
            {
                g.FillPath(trackBrush, trackPath);
            }

            // Fill
            if (_value > 0)
            {
                float fillWidth = Math.Max(Height, (float)_value / _maximum * Width);
                var fillRect = new Rectangle(0, 0, (int)fillWidth, Height - 1);

                using var fillPath = CreateRoundedRect(fillRect, radius);
                using var fillBrush = new LinearGradientBrush(
                    fillRect,
                    ThemeColors.AccentBlue,
                    Color.FromArgb(99, 160, 255),
                    LinearGradientMode.Horizontal);

                g.FillPath(fillBrush, fillPath);

                // Shine overlay
                var shineRect = new Rectangle(0, 0, (int)fillWidth, Height / 2);
                using var shineBrush = new SolidBrush(Color.FromArgb(25, 255, 255, 255));
                g.FillRectangle(shineBrush, shineRect);
            }

            // Status text
            string displayText = !string.IsNullOrEmpty(_statusText)
                ? _statusText
                : (_value > 0 ? $"{_value}%" : "Ready");

            TextRenderer.DrawText(g, displayText, ThemeColors.FontSmall, rect,
                ThemeColors.TextPrimary,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        private static GraphicsPath CreateRoundedRect(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int diameter = radius * 2;
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }
    }

    /// <summary>
    /// Themed panel with rounded corners and optional border.
    /// Used as card/section containers.
    /// </summary>
    public class ThemedPanel : Panel
    {
        private int _cornerRadius = 10;
        private bool _showBorder = true;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int CornerRadius
        {
            get => _cornerRadius;
            set { _cornerRadius = value; Invalidate(); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ShowBorder
        {
            get => _showBorder;
            set { _showBorder = value; Invalidate(); }
        }

        public ThemedPanel()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            BackColor = ThemeColors.BackgroundCard;
            Padding = new Padding(16, 12, 16, 12);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            using var path = CreateRoundedRect(rect, _cornerRadius);

            // Fill
            using var brush = new SolidBrush(BackColor);
            g.FillPath(brush, path);

            // Border
            if (_showBorder)
            {
                using var pen = new Pen(ThemeColors.Border, 1f);
                g.DrawPath(pen, path);
            }
        }

        private static GraphicsPath CreateRoundedRect(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int diameter = radius * 2;
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
