using GitBranchViewer.Core.Config;
using GitBranchViewer.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace GitBranchViewer.Core.Services
{
    /// <summary>
    /// Service that compares two Git branches and returns file and commit differences.
    /// Supports both direct repo comparison and diff via temporary merge repo.
    /// </summary>
    public class BranchComparisonService
    {
        private readonly GitService _gitService;
        private readonly DiffOptions _options;
        private readonly MergeRepoManager? _mergeRepoManager;

        public BranchComparisonService(GitService gitService, DiffOptions options, MergeRepoManager? mergeRepoManager = null)
        {
            _gitService = gitService;
            _options = options;
            _mergeRepoManager = mergeRepoManager;
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

        /// <summary>
        /// Performs full diff comparison via isolated temporary merge repository.
        /// This allows working outside of tracked GitService instances.
        /// </summary>
        public string CompareViaMergeRepo(string remoteUrl, string sourceBranch, string targetBranch)
        {
            if (_mergeRepoManager == null)
                throw new InvalidOperationException("MergeRepoManager not configured.");

            _mergeRepoManager.PrepareCleanMergeRepo(remoteUrl);
            _mergeRepoManager.FetchAndCheckoutBranch(sourceBranch, "BranchA");
            _mergeRepoManager.FetchAndCheckoutBranch(targetBranch, "BranchB");

            string pathA = _mergeRepoManager.GetWorktreePath("BranchA");
            string pathB = _mergeRepoManager.GetWorktreePath("BranchB");

            return RunRawDiff(pathA, pathB);
        }

        private static string RunRawDiff(string pathA, string pathB)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "diff",
                Arguments = $"-ru \"{pathA}\" \"{pathB}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0 && string.IsNullOrWhiteSpace(output))
                throw new InvalidOperationException($"diff failed:\n{error}");

            return output;
        }
    }
}