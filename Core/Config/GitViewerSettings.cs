using System.Collections.Generic;

namespace GitBranchViewer.Core.Config
{
    public class GitViewerSettings
    {
        public List<GitRepositoryConfig> GitRepositories { get; set; } = new();
        public DiffOptions DefaultDiffOptions { get; set; } = new();
    }
}