namespace GitBranchViewer.Core.Config
{
    /// <summary>
    /// Represents a Git branch configuration loaded from settings.
    /// </summary>
    public class BranchConfig
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
    }
}