using System.Collections.Generic;

namespace GitBranchViewer.Core.Config
{
    /// <summary>
    /// Represents a Git repository configuration loaded from settings.
    /// </summary>
    public class GitRepositoryConfig
    {
        /// <summary>
        /// Friendly display name shown to user.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Local file system path to repository root (must contain .git folder).
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Optional mapping of custom branch types, like "main" or "production".
        /// </summary>
        public Dictionary<string, string> BranchTypes { get; set; } = new();

        /// <summary>
        /// Remote Git URL used to initialize or sync merge folders.
        /// Example: https://github.com/user/GitBranchViewer.git
        /// </summary>
        public string RemoteUrl { get; set; }
    }
}