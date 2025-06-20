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
    }
}