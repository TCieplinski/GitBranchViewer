using System.Collections.Generic;

namespace GitBranchViewer.Core.Config
{
    public class GitViewerSettings
    {
        /// <summary>
        /// Repositories tracked by the application, each with its own configuration.
        /// </summary>
        public List<GitRepositoryConfig> GitRepositories { get; set; } = new();

        /// <summary>
        /// Default diffing options used when comparing repositories.
        /// </summary>
        public DiffOptions DefaultDiffOptions { get; set; } = new();

        /// <summary>
        /// Optional root path where temporary merge folders will be created.
        /// Defaults to AppData\GitBranchViewer\GitBranchViewerMerge if not set.
        /// </summary>
        public string? MergeFolderRootPath { get; set; }

        /// <summary>
        /// Name of the subdirectory to use under the root path for temporary merge folders.
        /// Defaults to "Merge".
        /// </summary>
        public string MergeSubfolderName { get; set; } = "Merge";

        /// <summary>
        /// Optional override for the temp folder used by remote-only diff (Method 3).
        /// If null, Path.GetTempPath() is used.
        /// </summary>
        public string? RemoteTempFolderRoot { get; set; }

        public string DiffToolPath { get; set; } = @"C:\Program Files\Git\usr\bin\diff.exe";
    }
}