using System;
using System.IO;
using GitBranchViewer.Core.Config;

namespace GitBranchViewer.Core.Services
{
    public class MergeRepoManager
    {
        private readonly string _mergeFolderPath;

        public MergeRepoManager(GitViewerSettings settings, string repositoryName)
        {
            var baseRoot = settings.MergeFolderRootPath
                ?? Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            if (!Directory.Exists(baseRoot))
                Directory.CreateDirectory(baseRoot); // ensure base path exists

            _mergeFolderPath = Path.Combine(baseRoot, repositoryName, settings.MergeSubfolderName);

            if (Directory.Exists(_mergeFolderPath))
            {
                bool deleted = TryDeleteWithRetries(_mergeFolderPath);

                if (!deleted)
                {
                    string fallbackPath = _mergeFolderPath + "_stale_" + DateTime.Now.Ticks;
                    Directory.Move(_mergeFolderPath, fallbackPath);
                    // Optionally mark for cleanup later
                }
            }

            Directory.CreateDirectory(_mergeFolderPath); // Re-create fresh folder
        }

        public void CleanMergeFolder()
        {
            if (Directory.Exists(_mergeFolderPath))
                Directory.Delete(_mergeFolderPath, true);
        }

        public void FetchAndCheckoutBranch(string branchName, string targetFolderName)
        {
            string targetPath = Path.Combine(_mergeFolderPath, targetFolderName);
            GitService.AddWorktree(_mergeFolderPath, targetPath, branchName);
        }

        public string GetWorktreePath(string branchName)
            => Path.Combine(_mergeFolderPath, branchName);

        public string GetMergeFolderPath() => _mergeFolderPath;

        private bool TryDeleteWithRetries(string path, int retries = 3, int delayMs = 150)
        {
            for (int i = 0; i < retries; i++)
            {
                try
                {
                    Directory.Delete(path, true);
                    return true;
                }
                catch
                {
                    System.Threading.Thread.Sleep(delayMs);
                }
            }
            return false;
        }
    }
}