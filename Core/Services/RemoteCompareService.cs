using GitBranchViewer.Core.Config;
using GitBranchViewer.Core.Models;
using System;
using System.Collections.Generic;

namespace GitBranchViewer.Core.Services
{
    /// <summary>
    /// Provides branch comparison and on-demand diff capabilities for remote-only repositories.
    /// </summary>
    public class RemoteCompareService
    {
        private readonly string? _tempRoot;

        public RemoteCompareService(string? remoteTempFolderRoot)
        {
            _tempRoot = remoteTempFolderRoot;
        }

        public List<GitFileChange> CompareBranches(string remoteUrl, string branchA, string branchB)
        {
            using var git = new RemoteGitService(remoteUrl, _tempRoot);
            return git.GetChangedFilesBetweenBranches(branchA, branchB);
        }

        public string GetFileDiff(string remoteUrl, string branchA, string branchB, string filePath)
        {
            using var git = new RemoteGitService(remoteUrl, _tempRoot);

            git.FetchBranch(branchA);
            git.FetchBranch(branchB);

            string contentA = string.Empty;
            string contentB = string.Empty;

            try
            {
                contentA = git.GetFileBlob(branchA, filePath);
            }
            catch (Exception)
            {
                // File might not exist in branchA (e.g. new file)
                contentA = string.Empty;
            }

            try
            {
                contentB = git.GetFileBlob(branchB, filePath);
            }
            catch (Exception)
            {
                // File might not exist in branchB (e.g. deleted file)
                contentB = string.Empty;
            }

            return DiffStrings(contentA, contentB);
        }

        private string DiffStrings(string a, string b)
        {
            var linesA = a.Replace("\r", "").Split('\n');
            var linesB = b.Replace("\r", "").Split('\n');

            var diff = new System.Text.StringBuilder();
            diff.AppendLine("--- A");
            diff.AppendLine("+++ B");

            for (int i = 0; i < Math.Max(linesA.Length, linesB.Length); i++)
            {
                string lineA = i < linesA.Length ? linesA[i] : "";
                string lineB = i < linesB.Length ? linesB[i] : "";

                if (lineA != lineB)
                {
                    if (!string.IsNullOrEmpty(lineA)) diff.AppendLine($"- {lineA}");
                    if (!string.IsNullOrEmpty(lineB)) diff.AppendLine($"+ {lineB}");
                }
            }

            return diff.ToString();
        }
    }
}