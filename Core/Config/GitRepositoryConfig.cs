using System.Collections.Generic;

namespace GitBranchViewer.Core.Config
{
    /// <summary>
    /// Represents a Git repository configuration loaded from settings.
    /// </summary>
    public class GitRepositoryConfig
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public Dictionary<string, string> BranchTypes { get; set; } = new();
    }
}