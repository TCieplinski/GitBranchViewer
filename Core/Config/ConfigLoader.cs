using Microsoft.Extensions.Configuration;
using System.IO;

namespace GitBranchViewer.Core.Config
{
    /// <summary>
    /// Loads Git repository viewer settings from appsettings.json.
    /// </summary>
    public static class ConfigLoader
    {
        public static GitViewerSettings Load(string jsonPath = "appsettings.json")
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(jsonPath, optional: false, reloadOnChange: false);

            var configuration = builder.Build();

            var settings = new GitViewerSettings();
            configuration.Bind(settings);

            return settings;
        }
    }
}