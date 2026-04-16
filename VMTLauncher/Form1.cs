using System.Diagnostics;

namespace VMTLauncher
{
    public partial class Form1 : Form
    {
        // ─── State ───────────────────────────────────────────────────
        private AppSettings _settings = null!;
        private List<ZipFileInfo> _zipFiles = new();
        private CancellationTokenSource? _extractCts;
        private bool _isExtracting = false;

        // ═══════════════════════════════════════════════════════════════
        //  INITIALIZATION
        // ═══════════════════════════════════════════════════════════════

        public Form1()
        {
            InitializeComponent();
            LoadSettings();
            WireEvents();
            RefreshUI();
        }

        /// <summary>
        /// Load saved settings and populate path fields.
        /// </summary>
        private void LoadSettings()
        {
            _settings = AppSettings.Load();
            txtMasterPath.Text = _settings.MasterPath;
            txtAppPath.Text = _settings.AppPath;
        }

        /// <summary>
        /// Wire up all event handlers.
        /// </summary>
        private void WireEvents()
        {
            // Browse buttons
            btnBrowseMaster.Click += BtnBrowseMaster_Click;
            btnBrowseApp.Click += BtnBrowseApp_Click;

            // Path text changes — auto-save & refresh
            txtMasterPath.TextChanged += TxtMasterPath_TextChanged;
            txtAppPath.TextChanged += TxtAppPath_TextChanged;

            // Version selector
            cmbVersions.SelectedIndexChanged += CmbVersions_SelectedIndexChanged;

            // Action buttons
            btnUpdate.Click += BtnUpdate_Click;
            btnOpenFolder.Click += BtnOpenFolder_Click;
            btnStartApp.Click += BtnStartApp_Click;

            // Form closing — save settings
            FormClosing += Form1_FormClosing;
        }

        // ═══════════════════════════════════════════════════════════════
        //  PATH BROWSING
        // ═══════════════════════════════════════════════════════════════

        private void BtnBrowseMaster_Click(object? sender, EventArgs e)
        {
            using var dialog = new FolderBrowserDialog
            {
                Description = "Select Master Path (folder containing .zip files)",
                UseDescriptionForTitle = true,
                ShowNewFolderButton = false,
            };

            if (!string.IsNullOrWhiteSpace(txtMasterPath.Text) && Directory.Exists(txtMasterPath.Text))
                dialog.InitialDirectory = txtMasterPath.Text;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                txtMasterPath.Text = dialog.SelectedPath;
            }
        }

        private void BtnBrowseApp_Click(object? sender, EventArgs e)
        {
            using var dialog = new FolderBrowserDialog
            {
                Description = "Select App Path (installation directory)",
                UseDescriptionForTitle = true,
                ShowNewFolderButton = true,
            };

            if (!string.IsNullOrWhiteSpace(txtAppPath.Text) && Directory.Exists(txtAppPath.Text))
                dialog.InitialDirectory = txtAppPath.Text;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                txtAppPath.Text = dialog.SelectedPath;
            }
        }

        // ═══════════════════════════════════════════════════════════════
        //  PATH CHANGE HANDLERS
        // ═══════════════════════════════════════════════════════════════

        private void TxtMasterPath_TextChanged(object? sender, EventArgs e)
        {
            _settings.MasterPath = txtMasterPath.Text;
            SaveSettings();
            ScanZipFilesAndPopulate();
        }

        private void TxtAppPath_TextChanged(object? sender, EventArgs e)
        {
            _settings.AppPath = txtAppPath.Text;
            SaveSettings();
            RefreshCurrentVersion();
        }

        // ═══════════════════════════════════════════════════════════════
        //  VERSION SCANNING & SELECTION
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// Scan master path for .zip files and populate the ComboBox.
        /// </summary>
        private void ScanZipFilesAndPopulate()
        {
            cmbVersions.Items.Clear();
            _zipFiles = UpdateService.ScanZipFiles(_settings.MasterPath);

            if (_zipFiles.Count == 0)
            {
                cmbVersions.Items.Add("No versions found");
                cmbVersions.SelectedIndex = 0;
                cmbVersions.Enabled = false;
                rtbPatchNotes.Text = "No .zip files found in the Master Path.\nPlease select a valid folder.";
                return;
            }

            cmbVersions.Enabled = true;
            foreach (var zip in _zipFiles)
            {
                cmbVersions.Items.Add(zip.ToString());
            }

            cmbVersions.SelectedIndex = 0; // Auto-select newest
        }

        /// <summary>
        /// Load patch notes when a version is selected.
        /// </summary>
        private void CmbVersions_SelectedIndexChanged(object? sender, EventArgs e)
        {
            int idx = cmbVersions.SelectedIndex;
            if (idx < 0 || idx >= _zipFiles.Count) return;

            var selected = _zipFiles[idx];
            string notes = UpdateService.ReadPatchNotes(selected.FullPath, _settings.MasterPath);

            rtbPatchNotes.Text = notes;
            rtbPatchNotes.ForeColor = ThemeColors.TextPrimary;

            SetStatus($"Selected: {selected.DisplayName}  •  {selected.FormattedSize}");
        }

        // ═══════════════════════════════════════════════════════════════
        //  CURRENT VERSION
        // ═══════════════════════════════════════════════════════════════

        private void RefreshCurrentVersion()
        {
            if (string.IsNullOrWhiteSpace(_settings.AppPath) || !Directory.Exists(_settings.AppPath))
            {
                lblCurrentVersion.Text = "Not Installed";
                lblCurrentVersion.ForeColor = ThemeColors.TextMuted;
                return;
            }

            string version = UpdateService.ReadCurrentVersion(_settings.AppPath);
            lblCurrentVersion.Text = version;
            lblCurrentVersion.ForeColor = version == "Not Installed"
                ? ThemeColors.TextMuted
                : ThemeColors.AccentGreen;
        }

        // ═══════════════════════════════════════════════════════════════
        //  UPDATE / INSTALL
        // ═══════════════════════════════════════════════════════════════

        private async void BtnUpdate_Click(object? sender, EventArgs e)
        {
            // ─── Validation ──────────────────────────────────────────
            if (_isExtracting)
            {
                // Cancel current extraction
                _extractCts?.Cancel();
                return;
            }

            int idx = cmbVersions.SelectedIndex;
            if (idx < 0 || idx >= _zipFiles.Count)
            {
                ShowWarning("Please select a version to install.");
                return;
            }

            if (string.IsNullOrWhiteSpace(_settings.AppPath))
            {
                ShowWarning("Please set the App Path (destination folder).");
                return;
            }

            // ─── Safety: Check if app is running ─────────────────────
            if (UpdateService.IsAppRunning(_settings.ExecutableName))
            {
                ShowWarning(
                    $"⚠️  {_settings.ExecutableName} is currently running!\n\n" +
                    "Please close the application before updating to avoid\n" +
                    "file access conflicts (Access Denied errors).",
                    "Application Running");
                return;
            }

            var selected = _zipFiles[idx];

            // ─── Confirmation ────────────────────────────────────────
            var result = MessageBox.Show(
                $"Install version:\n{selected.DisplayName}\n\n" +
                $"Destination:\n{_settings.AppPath}\n\n" +
                "Existing files will be overwritten.\nContinue?",
                "Confirm Update",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes) return;

            // ─── Begin Extraction ────────────────────────────────────
            _isExtracting = true;
            _extractCts = new CancellationTokenSource();
            SetUIExtractingState(true);

            try
            {
                // Backup preserved user-data folders (e.g. DataSave)
                SetStatus("Preserving user data...");
                var preservedBackups = await Task.Run(
                    () => UpdateService.BackupPreservedFolders(_settings.AppPath));

                // Optional: Backup Managed folder
                SetStatus("Creating backup...");
                await Task.Run(() => UpdateService.BackupManagedFolder(_settings.AppPath));

                // Extract with progress reporting
                var progressReporter = new Progress<ExtractProgress>(p =>
                {
                    progressBar.Value = p.Percentage;
                    progressBar.StatusText = $"{p.Percentage}%  —  {p.CurrentFile}";
                    SetStatus($"Extracting {p.ProcessedCount}/{p.TotalCount}: {p.CurrentFile}");
                });

                SetStatus("Extracting files...");
                await UpdateService.ExtractUpdateAsync(
                    selected.FullPath,
                    _settings.AppPath,
                    progressReporter,
                    _extractCts.Token);

                // Restore preserved user-data folders
                SetStatus("Restoring user data...");
                await Task.Run(() => UpdateService.RestorePreservedFolders(preservedBackups));

                // Write version file
                UpdateService.WriteVersionFile(_settings.AppPath, selected.FileName);

                // Success
                progressBar.Value = 100;
                progressBar.StatusText = "✓ Complete";
                SetStatus($"✓  Successfully installed {selected.DisplayName}");
                RefreshCurrentVersion();

                MessageBox.Show(
                    $"Successfully installed:\n{selected.DisplayName}\n\n" +
                    "You can now start the application.",
                    "Update Complete",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (OperationCanceledException)
            {
                SetStatus("⚠  Update cancelled by user.");
                progressBar.Value = 0;
                progressBar.StatusText = "Cancelled";
            }
            catch (Exception ex)
            {
                SetStatus($"✖  Error: {ex.Message}");
                progressBar.Value = 0;
                progressBar.StatusText = "Error";

                MessageBox.Show(
                    $"Update failed:\n\n{ex.Message}\n\n" +
                    "Please check the zip file and try again.",
                    "Update Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                _isExtracting = false;
                _extractCts?.Dispose();
                _extractCts = null;
                SetUIExtractingState(false);
            }
        }

        // ═══════════════════════════════════════════════════════════════
        //  OPEN FOLDER
        // ═══════════════════════════════════════════════════════════════

        private void BtnOpenFolder_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_settings.AppPath) || !Directory.Exists(_settings.AppPath))
            {
                ShowWarning("App Path folder does not exist.\nPlease set a valid path first.");
                return;
            }

            try
            {
                Process.Start("explorer.exe", _settings.AppPath);
            }
            catch (Exception ex)
            {
                ShowWarning($"Failed to open folder:\n{ex.Message}");
            }
        }

        // ═══════════════════════════════════════════════════════════════
        //  START APP
        // ═══════════════════════════════════════════════════════════════

        private void BtnStartApp_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_settings.AppPath))
            {
                ShowWarning("Please set the App Path first.");
                return;
            }

            string exePath = Path.Combine(_settings.AppPath, _settings.ExecutableName);

            if (!File.Exists(exePath))
            {
                ShowWarning(
                    $"Executable not found:\n{exePath}\n\n" +
                    "Please install a version first, or check Settings → ExecutableName\n" +
                    $"(currently: {_settings.ExecutableName})");
                return;
            }

            try
            {
                SetStatus($"Launching {_settings.ExecutableName}...");

                var startInfo = new ProcessStartInfo
                {
                    FileName = exePath,
                    WorkingDirectory = _settings.AppPath,
                    UseShellExecute = true,
                };

                Process.Start(startInfo);
                SetStatus($"✓  {_settings.ExecutableName} launched successfully.");
            }
            catch (Exception ex)
            {
                ShowWarning($"Failed to launch application:\n{ex.Message}");
            }
        }

        // ═══════════════════════════════════════════════════════════════
        //  UI HELPERS
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// Refresh all UI elements.
        /// </summary>
        private void RefreshUI()
        {
            ScanZipFilesAndPopulate();
            RefreshCurrentVersion();
        }

        /// <summary>
        /// Toggle UI state during extraction (disable controls to prevent conflicts).
        /// </summary>
        private void SetUIExtractingState(bool extracting)
        {
            btnUpdate.Text = extracting ? "✖  CANCEL" : "⬇  UPDATE / INSTALL";
            btnUpdate.BaseColor = extracting ? ThemeColors.AccentRed : ThemeColors.AccentBlue;
            btnUpdate.HoverColor = extracting
                ? ControlPaint.Dark(ThemeColors.AccentRed, 0.1f)
                : ThemeColors.AccentBlueHover;

            btnStartApp.Enabled = !extracting;
            btnOpenFolder.Enabled = !extracting;
            cmbVersions.Enabled = !extracting;
            txtMasterPath.Enabled = !extracting;
            txtAppPath.Enabled = !extracting;
            btnBrowseMaster.Enabled = !extracting;
            btnBrowseApp.Enabled = !extracting;

            if (!extracting)
            {
                progressBar.Value = 0;
                progressBar.StatusText = "";
            }
        }

        /// <summary>
        /// Update the status label text.
        /// </summary>
        private void SetStatus(string text)
        {
            lblStatus.Text = text;
        }

        /// <summary>
        /// Save settings to disk.
        /// </summary>
        private void SaveSettings()
        {
            _settings.Save();
        }

        /// <summary>
        /// Show a warning message dialog.
        /// </summary>
        private static void ShowWarning(string message, string title = "Warning")
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        // ═══════════════════════════════════════════════════════════════
        //  FORM EVENTS
        // ═══════════════════════════════════════════════════════════════

        private void Form1_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (_isExtracting)
            {
                var result = MessageBox.Show(
                    "An extraction is in progress.\nAre you sure you want to exit?",
                    "Extraction In Progress",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }

                _extractCts?.Cancel();
            }

            SaveSettings();
        }
    }
}
