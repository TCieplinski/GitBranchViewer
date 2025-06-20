using System;
using System.IO;
using GitBranchViewer.Core.Config;

namespace GitBranchViewer.Core.Services
{
    public class MergeRepoManager
    {
        private readonly string _mergeFolderPath;

        public MergeRepoManager(GitViewerSettings settings)
        {
            _mergeFolderPath = settings.MergeFolderRootPath ??
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GitBranchViewer", "GitBranchViewerMerge");
        }

        public void PrepareCleanMergeRepo(string remoteUrl)
        {
            if (Directory.Exists(_mergeFolderPath))
                Directory.Delete(_mergeFolderPath, true);

            Directory.CreateDirectory(_mergeFolderPath);
            GitService.InitRepository(_mergeFolderPath, remoteUrl);
        }

        public void FetchAndCheckoutBranch(string branchName, string targetFolderName)
        {
            string targetPath = Path.Combine(Path.GetDirectoryName(_mergeFolderPath)!, targetFolderName);
            GitService.AddWorktree(_mergeFolderPath, targetPath, branchName);
        }

        public string GetWorktreePath(string branchName)
            => Path.Combine(Path.GetDirectoryName(_mergeFolderPath)!, branchName);
    }
}