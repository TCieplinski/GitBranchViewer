using System.Collections.Generic;

namespace GitBranchViewer.Core.Config
{
    /// <summary>
    /// Represents a Git repository configuration loaded from settings.
    /// </summary>
    public class GitRepositoryConfig
    {
        /// <summary>
        /// Display name shown in UI.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Local path to the repository (optional if RemoteOnly = true).
        /// </summary>
        public string? Path { get; set; }

        /// <summary>
        /// Remote Git URL used for fetches or remote-only comparisons.
        /// </summary>
        public string RemoteUrl { get; set; }

        /// <summary>
        /// List of semantic type to branch mappings (e.g. feature to v1.0.0).
        /// Allows duplicates for one-to-many relationships.
        /// </summary>
        public List<BranchConfig> Branches { get; set; } = new();

        /// <summary>
        /// If true, Path is ignored and all Git operations are performed remotely or via temporary clone.
        /// </summary>
        public bool IsMultiBranch { get; set; } = false;
    }
}