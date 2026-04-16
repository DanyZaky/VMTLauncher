using System.Text.Json;

namespace VMTLauncher
{
    /// <summary>
    /// Manages persistent application settings (replaces Properties.Settings for .NET 6+).
    /// Settings are stored as a JSON file in the application directory.
    /// </summary>
    public class AppSettings
    {
        private static readonly string SettingsFilePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "launcher_settings.json");

        public string MasterPath { get; set; } = string.Empty;
        public string AppPath { get; set; } = string.Empty;
        public string ExecutableName { get; set; } = "VMT Editor.exe";

        /// <summary>
        /// Load settings from disk. Returns default settings if file doesn't exist.
        /// </summary>
        public static AppSettings Load()
        {
            try
            {
                if (File.Exists(SettingsFilePath))
                {
                    string json = File.ReadAllText(SettingsFilePath);
                    return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AppSettings] Load failed: {ex.Message}");
            }

            return new AppSettings();
        }

        /// <summary>
        /// Save current settings to disk.
        /// </summary>
        public void Save()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(this, options);
                File.WriteAllText(SettingsFilePath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AppSettings] Save failed: {ex.Message}");
            }
        }
    }
}
