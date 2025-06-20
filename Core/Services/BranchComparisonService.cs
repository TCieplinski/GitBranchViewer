using GitBranchViewer.Core.Config;
using GitBranchViewer.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace GitBranchViewer.Core.Services
{
    /// <summary>
    /// Service that compares two Git branches and returns file and commit differences.
    /// </summary>
    public class BranchComparisonService
    {
        private readonly GitService _gitService;
        private readonly DiffOptions _options;

        public BranchComparisonService(GitService gitService, DiffOptions options)
        {
            _gitService = gitService;
            _options = options;
        }

        public List<GitFileChange> GetChangedFiles(string branchA, string branchB)
        {
            var files = _gitService.GetChangedFilesBetweenBranches(branchA, branchB);

            if (_options.ShowOnlyChangedFiles)
            {
                files = files
                    .Where(line => line.Status.StartsWith("M") || line.Status.StartsWith("A") || line.Status.StartsWith("D"))
                    .ToList();
            }

            return files;
        }

        public List<(string Hash, string Author, string Date, string Message)> GetCommitDifferences(string branchA, string branchB)
        {
            return _gitService.GetCommits($"{branchA}..{branchB}");
        }

        public string GetFileDiff(string filePath, string branchA, string branchB)
        {
            if (_options.IgnoreWhitespace)
                return _gitService.GetFileDiff(filePath, branchA, branchB + " --ignore-space-change");
            else
                return _gitService.GetFileDiff(filePath, branchA, branchB);
        }
    }
}