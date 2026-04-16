#nullable enable
using System.Drawing.Drawing2D;
using VMTLauncher.Controls;

namespace VMTLauncher
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer? components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        // ─── Header ──────────────────────────────────────────────────
        private Panel pnlHeader = null!;
        private Label lblTitle = null!;
        private Label lblSubtitle = null!;

        // ─── Configuration Section ───────────────────────────────────
        private ThemedPanel pnlConfig = null!;
        private Label lblMasterPath = null!;
        private TextBox txtMasterPath = null!;
        private ThemedButton btnBrowseMaster = null!;
        private Label lblAppPath = null!;
        private TextBox txtAppPath = null!;
        private ThemedButton btnBrowseApp = null!;

        // ─── Version Selector ────────────────────────────────────────
        private ThemedPanel pnlVersion = null!;
        private Label lblSelectVersion = null!;
        private ComboBox cmbVersions = null!;
        private Label lblCurrentVersionTitle = null!;
        private Label lblCurrentVersion = null!;

        // ─── Patch Notes ─────────────────────────────────────────────
        private ThemedPanel pnlPatchNotes = null!;
        private Label lblPatchNotesTitle = null!;
        private RichTextBox rtbPatchNotes = null!;

        // ─── Progress ────────────────────────────────────────────────
        private ThemedProgressBar progressBar = null!;
        private Label lblStatus = null!;

        // ─── Action Buttons ──────────────────────────────────────────
        private Panel pnlActions = null!;
        private ThemedButton btnUpdate = null!;
        private ThemedButton btnOpenFolder = null!;
        private ThemedButton btnStartApp = null!;

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            // ══════════════════════════════════════════════════════════
            //  FORM CONFIGURATION
            // ══════════════════════════════════════════════════════════
            SuspendLayout();
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(680, 780);
            MinimumSize = new Size(650, 750);
            Text = "VMT Launcher";
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = ThemeColors.BackgroundDark;
            ForeColor = ThemeColors.TextPrimary;
            Font = ThemeColors.FontBody;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            DoubleBuffered = true;

            int contentWidth = 640;
            int leftMargin = 20;

            // ══════════════════════════════════════════════════════════
            //  HEADER
            // ══════════════════════════════════════════════════════════
            pnlHeader = new Panel
            {
                Name = "pnlHeader",
                Location = new Point(0, 0),
                Size = new Size(ClientSize.Width, 80),
                BackColor = ThemeColors.BackgroundMain,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            };

            lblTitle = new Label
            {
                Name = "lblTitle",
                Text = "⬡  VMT LAUNCHER",
                Font = ThemeColors.FontTitle,
                ForeColor = ThemeColors.TextHighlight,
                Location = new Point(leftMargin + 4, 16),
                AutoSize = true,
                BackColor = Color.Transparent,
            };

            lblSubtitle = new Label
            {
                Name = "lblSubtitle",
                Text = "Version Management & Deployment Tool",
                Font = ThemeColors.FontSubtitle,
                ForeColor = ThemeColors.TextMuted,
                Location = new Point(leftMargin + 8, 50),
                AutoSize = true,
                BackColor = Color.Transparent,
            };

            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblSubtitle);
            pnlHeader.Paint += PnlHeader_Paint;

            // ══════════════════════════════════════════════════════════
            //  CONFIGURATION SECTION
            // ══════════════════════════════════════════════════════════
            pnlConfig = new ThemedPanel
            {
                Name = "pnlConfig",
                Location = new Point(leftMargin, 92),
                Size = new Size(contentWidth, 130),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            };

            // Master Path
            lblMasterPath = new Label
            {
                Name = "lblMasterPath",
                Text = "📁  MASTER PATH (SOURCE)",
                Font = ThemeColors.FontLabel,
                ForeColor = ThemeColors.TextSecondary,
                Location = new Point(16, 12),
                AutoSize = true,
                BackColor = Color.Transparent,
            };

            txtMasterPath = new TextBox
            {
                Name = "txtMasterPath",
                Location = new Point(16, 34),
                Size = new Size(contentWidth - 120, 28),
                Font = ThemeColors.FontBody,
                BackColor = ThemeColors.BackgroundInput,
                ForeColor = ThemeColors.TextPrimary,
                BorderStyle = BorderStyle.FixedSingle,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            };

            btnBrowseMaster = new ThemedButton
            {
                Name = "btnBrowseMaster",
                Text = "Browse",
                Location = new Point(contentWidth - 96, 33),
                Size = new Size(80, 30),
                BaseColor = ThemeColors.BackgroundHover,
                HoverColor = ThemeColors.BorderLight,
                CornerRadius = 6,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
            };

            // App Path
            lblAppPath = new Label
            {
                Name = "lblAppPath",
                Text = "💾  APP PATH (DESTINATION)",
                Font = ThemeColors.FontLabel,
                ForeColor = ThemeColors.TextSecondary,
                Location = new Point(16, 70),
                AutoSize = true,
                BackColor = Color.Transparent,
            };

            txtAppPath = new TextBox
            {
                Name = "txtAppPath",
                Location = new Point(16, 92),
                Size = new Size(contentWidth - 120, 28),
                Font = ThemeColors.FontBody,
                BackColor = ThemeColors.BackgroundInput,
                ForeColor = ThemeColors.TextPrimary,
                BorderStyle = BorderStyle.FixedSingle,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            };

            btnBrowseApp = new ThemedButton
            {
                Name = "btnBrowseApp",
                Text = "Browse",
                Location = new Point(contentWidth - 96, 91),
                Size = new Size(80, 30),
                BaseColor = ThemeColors.BackgroundHover,
                HoverColor = ThemeColors.BorderLight,
                CornerRadius = 6,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
            };

            pnlConfig.Controls.AddRange(new Control[] {
                lblMasterPath, txtMasterPath, btnBrowseMaster,
                lblAppPath, txtAppPath, btnBrowseApp
            });

            // ══════════════════════════════════════════════════════════
            //  VERSION SELECTOR
            // ══════════════════════════════════════════════════════════
            pnlVersion = new ThemedPanel
            {
                Name = "pnlVersion",
                Location = new Point(leftMargin, 232),
                Size = new Size(contentWidth, 70),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            };

            lblSelectVersion = new Label
            {
                Name = "lblSelectVersion",
                Text = "📦  SELECT VERSION",
                Font = ThemeColors.FontLabel,
                ForeColor = ThemeColors.TextSecondary,
                Location = new Point(16, 12),
                AutoSize = true,
                BackColor = Color.Transparent,
            };

            cmbVersions = new ComboBox
            {
                Name = "cmbVersions",
                Location = new Point(16, 34),
                Size = new Size(contentWidth - 280, 28),
                Font = ThemeColors.FontBody,
                BackColor = ThemeColors.BackgroundInput,
                ForeColor = ThemeColors.TextPrimary,
                FlatStyle = FlatStyle.Flat,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            };

            lblCurrentVersionTitle = new Label
            {
                Name = "lblCurrentVersionTitle",
                Text = "INSTALLED:",
                Font = ThemeColors.FontSmall,
                ForeColor = ThemeColors.TextMuted,
                Location = new Point(contentWidth - 250, 18),
                AutoSize = true,
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
            };

            lblCurrentVersion = new Label
            {
                Name = "lblCurrentVersion",
                Text = "Not Installed",
                Font = ThemeColors.FontVersion,
                ForeColor = ThemeColors.AccentGreen,
                Location = new Point(contentWidth - 250, 34),
                Size = new Size(234, 28),
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
            };

            pnlVersion.Controls.AddRange(new Control[] {
                lblSelectVersion, cmbVersions,
                lblCurrentVersionTitle, lblCurrentVersion
            });

            // ══════════════════════════════════════════════════════════
            //  PATCH NOTES
            // ══════════════════════════════════════════════════════════
            pnlPatchNotes = new ThemedPanel
            {
                Name = "pnlPatchNotes",
                Location = new Point(leftMargin, 312),
                Size = new Size(contentWidth, 320),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
            };

            lblPatchNotesTitle = new Label
            {
                Name = "lblPatchNotesTitle",
                Text = "📝  PATCH NOTES",
                Font = ThemeColors.FontLabel,
                ForeColor = ThemeColors.TextSecondary,
                Location = new Point(16, 12),
                AutoSize = true,
                BackColor = Color.Transparent,
            };

            rtbPatchNotes = new RichTextBox
            {
                Name = "rtbPatchNotes",
                Location = new Point(16, 36),
                Size = new Size(contentWidth - 32, 272),
                Font = ThemeColors.FontPatchNotes,
                BackColor = ThemeColors.BackgroundInput,
                ForeColor = ThemeColors.TextPrimary,
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                ScrollBars = RichTextBoxScrollBars.Vertical,
                Text = "Select a version to view patch notes...",
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
            };

            pnlPatchNotes.Controls.AddRange(new Control[] {
                lblPatchNotesTitle, rtbPatchNotes
            });

            // ══════════════════════════════════════════════════════════
            //  PROGRESS BAR & STATUS
            // ══════════════════════════════════════════════════════════
            progressBar = new ThemedProgressBar
            {
                Name = "progressBar",
                Location = new Point(leftMargin, 644),
                Size = new Size(contentWidth, 26),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
            };

            lblStatus = new Label
            {
                Name = "lblStatus",
                Text = "Ready",
                Font = ThemeColors.FontSmall,
                ForeColor = ThemeColors.TextMuted,
                Location = new Point(leftMargin + 4, 674),
                Size = new Size(contentWidth, 20),
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
            };

            // ══════════════════════════════════════════════════════════
            //  ACTION BUTTONS
            // ══════════════════════════════════════════════════════════
            pnlActions = new Panel
            {
                Name = "pnlActions",
                Location = new Point(leftMargin, 700),
                Size = new Size(contentWidth, 50),
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
            };

            int btnWidth = (contentWidth - 20) / 3;

            btnUpdate = new ThemedButton
            {
                Name = "btnUpdate",
                Text = "⬇  UPDATE / INSTALL",
                Size = new Size(btnWidth, 44),
                Location = new Point(0, 0),
                BaseColor = ThemeColors.AccentBlue,
                HoverColor = ThemeColors.AccentBlueHover,
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
            };

            btnOpenFolder = new ThemedButton
            {
                Name = "btnOpenFolder",
                Text = "📂  OPEN FOLDER",
                Size = new Size(btnWidth, 44),
                Location = new Point(btnWidth + 10, 0),
                BaseColor = ThemeColors.BackgroundHover,
                HoverColor = ThemeColors.BorderLight,
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
            };

            btnStartApp = new ThemedButton
            {
                Name = "btnStartApp",
                Text = "▶  START APP",
                Size = new Size(btnWidth, 44),
                Location = new Point((btnWidth + 10) * 2, 0),
                BaseColor = ThemeColors.AccentGreen,
                HoverColor = ThemeColors.AccentGreenHover,
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
            };

            pnlActions.Controls.AddRange(new Control[] {
                btnUpdate, btnOpenFolder, btnStartApp
            });

            // ══════════════════════════════════════════════════════════
            //  ADD ALL TO FORM
            // ══════════════════════════════════════════════════════════
            Controls.AddRange(new Control[] {
                pnlHeader,
                pnlConfig,
                pnlVersion,
                pnlPatchNotes,
                progressBar,
                lblStatus,
                pnlActions,
            });

            ResumeLayout(false);
            PerformLayout();
        }

        /// <summary>
        /// Paints the header with a subtle gradient bottom border.
        /// </summary>
        private void PnlHeader_Paint(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            var rect = pnlHeader.ClientRectangle;

            // Bottom accent line (gradient)
            using var gradientBrush = new LinearGradientBrush(
                new Point(0, rect.Bottom - 2),
                new Point(rect.Width, rect.Bottom - 2),
                ThemeColors.AccentBlue,
                Color.FromArgb(0, ThemeColors.AccentBlue));

            g.FillRectangle(gradientBrush, 0, rect.Bottom - 2, rect.Width, 2);
        }

        #endregion
    }
}
