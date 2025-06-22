using System.Collections.Generic;
using System;
using System.IO;
using System.Text.Json;
using GitBranchViewer.Core.Config;

namespace GitBranchViewer.Core.Services
{
    public class SettingsService
    {
        private readonly string _settingsPath;

        public GitViewerSettings Current { get; private set; } = new();

        public SettingsService()
        {
            var baseFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "GitBranchViewer");

            if (!Directory.Exists(baseFolder))
                Directory.CreateDirectory(baseFolder);

            _settingsPath = Path.Combine(baseFolder, "user.settings.json");

            // Auto-load settings if present
            if (File.Exists(_settingsPath))
            {
                try
                {
                    var json = File.ReadAllText(_settingsPath);
                    Current = JsonSerializer.Deserialize<GitViewerSettings>(json) ?? new();
                }
                catch
                {
                    // Ignore errors silently, fallback to empty settings
                    Current = new GitViewerSettings();
                }
            }
        }

        public void Load()
        {
            if (File.Exists(_settingsPath))
            {
                var json = File.ReadAllText(_settingsPath);
                Current = JsonSerializer.Deserialize<GitViewerSettings>(json) ?? new();
            }
        }

        public void Save()
        {
            var json = JsonSerializer.Serialize(Current, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_settingsPath, json);
        }

        public void LoadFromJson(string json)
        {
            var parsed = JsonSerializer.Deserialize<GitViewerSettings>(json);
            if (parsed != null)
            {
                Current = parsed;
                Save();
            }
        }

        public string GetCurrentUserSettingsAsJson()
        {
            return JsonSerializer.Serialize(Current, new JsonSerializerOptions { WriteIndented = true });
        }

        public string GetJsonTemplate()
        {
            var template = new GitViewerSettings
            {
                GitRepositories = new List<GitRepositoryConfig>
                {
                    new GitRepositoryConfig
                    {
                        Name = "MyRepo",
                        Path = "~/repos/MyRepo",
                        Branches = new List<BranchConfig>()
                        {
                            new BranchConfig { Type = "main", Name = "Main" },
                            new BranchConfig { Type = "dev", Name = "Development" }
                        }
                    }
                },
                DefaultDiffOptions = new DiffOptions { IgnoreWhitespace = true, ShowOnlyChangedFiles = true }
            };

            return JsonSerializer.Serialize(template, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}