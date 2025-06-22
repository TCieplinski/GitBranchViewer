using System.IO;

namespace GitBranchViewer.Core.Services
{
    public static class GitServiceFactory
    {
        public static GitService CreateForLocal(string path)
        {
            return new GitService(path);
        }

        public static GitService CreateForMergeRepo(string mergeFolderPath, string remoteUrl)
        {
            if (!Directory.Exists(mergeFolderPath) || !Directory.Exists(Path.Combine(mergeFolderPath, ".git")))
            {
                Directory.CreateDirectory(mergeFolderPath);
                GitService.InitRepository(mergeFolderPath, remoteUrl);
            }

            return new GitService(mergeFolderPath, skipValidation: true);
        }
    }
}