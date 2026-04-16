using System.Diagnostics;
using System.IO.Compression;
using System.Text.Json;

namespace VMTLauncher
{
    /// <summary>
    /// Handles all update/extraction logic for the launcher.
    /// Provides async operations with progress reporting.
    /// </summary>
    public class UpdateService
    {
        /// <summary>
        /// Scans the master path for .zip files and returns them sorted by LastWriteTime (newest first).
        /// </summary>
        public static List<ZipFileInfo> ScanZipFiles(string masterPath)
        {
            if (string.IsNullOrWhiteSpace(masterPath) || !Directory.Exists(masterPath))
                return new List<ZipFileInfo>();

            var files = Directory.GetFiles(masterPath, "*.zip")
                .Select(f => new FileInfo(f))
                .OrderByDescending(f => f.LastWriteTime)
                .Select(f => new ZipFileInfo
                {
                    FileName = f.Name,
                    FullPath = f.FullName,
                    LastModified = f.LastWriteTime,
                    FileSize = f.Length
                })
                .ToList();

            return files;
        }

        /// <summary>
        /// Loads and caches the versions.json data from the master path.
        /// Returns null if file doesn't exist or is invalid.
        /// </summary>
        public static VersionsData? LoadVersionsJson(string masterPath)
        {
            if (string.IsNullOrWhiteSpace(masterPath) || !Directory.Exists(masterPath))
                return null;

            string jsonPath = Path.Combine(masterPath, "versions.json");
            if (!File.Exists(jsonPath))
                return null;

            try
            {
                string json = File.ReadAllText(jsonPath);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                return JsonSerializer.Deserialize<VersionsData>(json, options);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[UpdateService] Failed to read versions.json: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Reads the patch notes for a specific zip file.
        /// First tries to match from versions.json, then falls back to .txt file.
        /// </summary>
        public static string ReadPatchNotes(string zipFullPath, string masterPath)
        {
            string zipFileName = Path.GetFileNameWithoutExtension(zipFullPath);

            // Try versions.json first
            var versionsData = LoadVersionsJson(masterPath);
            if (versionsData?.Versions != null)
            {
                // Try to match version from zip filename
                // Supports patterns like: "VMT_Editor_v0.1.3", "VMT Editor v0.1.3", "app_v1.0.0"
                foreach (var entry in versionsData.Versions)
                {
                    if (string.IsNullOrEmpty(entry.Version)) continue;

                    // Check if the zip filename contains this version string
                    if (zipFileName.Contains(entry.Version, StringComparison.OrdinalIgnoreCase))
                    {
                        return FormatVersionEntry(entry);
                    }
                }
            }

            // Fallback: try .txt file with same name
            string txtPath = Path.ChangeExtension(zipFullPath, ".txt");
            if (File.Exists(txtPath))
            {
                try
                {
                    return File.ReadAllText(txtPath);
                }
                catch (Exception ex)
                {
                    return $"[Error reading patch notes]\n{ex.Message}";
                }
            }

            return "No patch notes available for this version.";
        }

        /// <summary>
        /// Formats a single VersionEntry into a human-readable patch notes string.
        /// </summary>
        private static string FormatVersionEntry(VersionEntry entry)
        {
            var lines = new List<string>();

            // Header
            lines.Add($"═══  Version {entry.Version}  ═══");
            if (!string.IsNullOrEmpty(entry.Date))
                lines.Add($"📅  {entry.Date}");
            lines.Add("");

            if (entry.Changes == null || entry.Changes.Count == 0)
            {
                lines.Add("No changes recorded.");
                return string.Join("\n", lines);
            }

            // Group changes by type
            var grouped = entry.Changes
                .GroupBy(c => (c.Type ?? "other").ToLowerInvariant())
                .OrderBy(g => GetTypeOrder(g.Key));

            foreach (var group in grouped)
            {
                string icon = GetTypeIcon(group.Key);
                string label = GetTypeLabel(group.Key);

                lines.Add($"{icon}  {label}");
                foreach (var change in group)
                {
                    lines.Add($"    •  {change.Value}");
                }
                lines.Add("");
            }

            return string.Join("\n", lines).TrimEnd();
        }

        /// <summary>
        /// Gets the display icon for a change type.
        /// </summary>
        private static string GetTypeIcon(string type) => type switch
        {
            "added" => "✨",
            "fixed" => "🔧",
            "updated" => "🔄",
            "removed" => "🗑️",
            _ => "📌"
        };

        /// <summary>
        /// Gets the display label for a change type.
        /// </summary>
        private static string GetTypeLabel(string type) => type switch
        {
            "added" => "ADDED",
            "fixed" => "FIXED",
            "updated" => "UPDATED",
            "removed" => "REMOVED",
            _ => type.ToUpperInvariant()
        };

        /// <summary>
        /// Gets the sort order for grouping change types.
        /// </summary>
        private static int GetTypeOrder(string type) => type switch
        {
            "added" => 0,
            "updated" => 1,
            "fixed" => 2,
            "removed" => 3,
            _ => 4
        };

        /// <summary>
        /// Reads the current installed version from version.txt in the app path.
        /// </summary>
        public static string ReadCurrentVersion(string appPath)
        {
            string versionFile = Path.Combine(appPath, "version.txt");

            if (File.Exists(versionFile))
            {
                try
                {
                    return File.ReadAllText(versionFile).Trim();
                }
                catch
                {
                    return "Unknown";
                }
            }

            return "Not Installed";
        }

        /// <summary>
        /// Checks whether the target executable is currently running.
        /// </summary>
        public static bool IsAppRunning(string exeName)
        {
            // Remove .exe extension for process name matching
            string processName = Path.GetFileNameWithoutExtension(exeName);
            return Process.GetProcessesByName(processName).Length > 0;
        }

        /// <summary>
        /// Extracts the selected zip file to the app path with progress reporting.
        /// Uses async/await to keep UI responsive.
        /// </summary>
        public static async Task ExtractUpdateAsync(
            string zipPath,
            string appPath,
            IProgress<ExtractProgress> progress,
            CancellationToken cancellationToken = default)
        {
            await Task.Run(() =>
            {
                using var archive = ZipFile.OpenRead(zipPath);
                int totalEntries = archive.Entries.Count;
                int processedEntries = 0;

                // Ensure target directory exists
                Directory.CreateDirectory(appPath);

                foreach (var entry in archive.Entries)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    string destinationPath = Path.GetFullPath(
                        Path.Combine(appPath, entry.FullName));

                    // Security: Prevent zip-slip attack
                    if (!destinationPath.StartsWith(
                        Path.GetFullPath(appPath) + Path.DirectorySeparatorChar,
                        StringComparison.OrdinalIgnoreCase) &&
                        !destinationPath.Equals(Path.GetFullPath(appPath),
                        StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException(
                            $"Zip entry '{entry.FullName}' has an invalid path.");
                    }

                    // If it's a directory entry, just create it
                    if (string.IsNullOrEmpty(entry.Name))
                    {
                        Directory.CreateDirectory(destinationPath);
                    }
                    else
                    {
                        // Ensure parent directory exists
                        string? parentDir = Path.GetDirectoryName(destinationPath);
                        if (parentDir != null)
                            Directory.CreateDirectory(parentDir);

                        // Extract with overwrite
                        entry.ExtractToFile(destinationPath, overwrite: true);
                    }

                    processedEntries++;
                    int percentage = (int)((double)processedEntries / totalEntries * 100);

                    progress.Report(new ExtractProgress
                    {
                        Percentage = percentage,
                        CurrentFile = entry.FullName,
                        ProcessedCount = processedEntries,
                        TotalCount = totalEntries
                    });
                }
            }, cancellationToken);
        }

        /// <summary>
        /// Writes the version identifier to version.txt in the app path.
        /// </summary>
        public static void WriteVersionFile(string appPath, string versionName)
        {
            string versionFile = Path.Combine(appPath, "version.txt");
            File.WriteAllText(versionFile, versionName);
        }

        /// <summary>
        /// Creates a backup of the Managed folder (optional safety measure).
        /// </summary>
        public static void BackupManagedFolder(string appPath)
        {
            string managedPath = Path.Combine(appPath, "Managed");
            string backupPath = Path.Combine(appPath, "Managed_Backup");

            if (Directory.Exists(managedPath))
            {
                // Remove old backup if it exists
                if (Directory.Exists(backupPath))
                {
                    Directory.Delete(backupPath, recursive: true);
                }

                Directory.Move(managedPath, backupPath);
            }
        }
    }

    // ═══════════════════════════════════════════════════════════════
    //  DATA MODELS
    // ═══════════════════════════════════════════════════════════════

    /// <summary>
    /// Root model for versions.json
    /// </summary>
    public class VersionsData
    {
        public List<VersionEntry>? Versions { get; set; }
    }

    /// <summary>
    /// A single version entry in versions.json
    /// </summary>
    public class VersionEntry
    {
        public string? Version { get; set; }
        public string? Date { get; set; }
        public List<ChangeEntry>? Changes { get; set; }
    }

    /// <summary>
    /// A single change item within a version entry.
    /// </summary>
    public class ChangeEntry
    {
        public string? Type { get; set; }
        public string? Value { get; set; }
    }

    /// <summary>
    /// Represents a discovered zip file in the master path.
    /// </summary>
    public class ZipFileInfo
    {
        public string FileName { get; set; } = string.Empty;
        public string FullPath { get; set; } = string.Empty;
        public DateTime LastModified { get; set; }
        public long FileSize { get; set; }

        public string DisplayName => Path.GetFileNameWithoutExtension(FileName);

        public string FormattedSize
        {
            get
            {
                if (FileSize < 1024) return $"{FileSize} B";
                if (FileSize < 1024 * 1024) return $"{FileSize / 1024.0:F1} KB";
                if (FileSize < 1024 * 1024 * 1024) return $"{FileSize / (1024.0 * 1024):F1} MB";
                return $"{FileSize / (1024.0 * 1024 * 1024):F2} GB";
            }
        }

        public override string ToString() => $"{DisplayName}  ({LastModified:yyyy-MM-dd HH:mm})  [{FormattedSize}]";
    }

    /// <summary>
    /// Progress reporting data for extraction operations.
    /// </summary>
    public class ExtractProgress
    {
        public int Percentage { get; set; }
        public string CurrentFile { get; set; } = string.Empty;
        public int ProcessedCount { get; set; }
        public int TotalCount { get; set; }
    }
}
