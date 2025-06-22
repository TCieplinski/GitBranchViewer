using GitBranchViewer.Core.Models;
using GitBranchViewer.Core.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace GitBranchViewer.Core.Services
{
    /// <summary>
    /// Provides shallow Git access to remote repositories without downloading full file contents.
    /// Supports comparing branches and on-demand blob extraction.
    /// </summary>
    public class RemoteGitService : IDisposable
    {
        private readonly string _workingPath;
        private readonly string _remoteUrl;

        public RemoteGitService(string remoteUrl, string? tempRootOverride = null)
        {
            _remoteUrl = remoteUrl;

            var tempRoot = string.IsNullOrEmpty(tempRootOverride)
                ? Path.GetTempPath()
                : FileHelper.ExpandHomePath(tempRootOverride);

            _workingPath = Path.Combine(tempRoot, "GitBranchViewer", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_workingPath);

            RunGit("init", _workingPath);
            RunGit($"remote add origin \"{remoteUrl}\"", _workingPath);
        }

        public void FetchBranch(string branch)
        {
            RunGit($"fetch --depth=1 --filter=blob:none origin {branch}", _workingPath);
        }

        public List<GitFileChange> GetChangedFilesBetweenBranches(string branchA, string branchB)
        {
            FetchBranch(branchA);
            FetchBranch(branchB);

            string output = RunGit($"diff --name-status origin/{branchA} origin/{branchB}", _workingPath);

            return output.Split('\n')
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line =>
                {
                    var parts = line.Trim().Split('\t', 2);
                    return new GitFileChange(parts[0], parts[1]);
                })
                .ToList();
        }

        public string GetFileBlob(string branch, string filePath)
        {
            return RunGit($"show origin/{branch}:{filePath}", _workingPath);
        }

        private static string RunGit(string arguments, string workingDirectory)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = arguments,
                WorkingDirectory = workingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
                throw new InvalidOperationException($"Git command failed: {arguments}\n{error}");

            return output;
        }

        public void Dispose()
        {
            try
            {
                if (Directory.Exists(_workingPath))
                    Directory.Delete(_workingPath, recursive: true);
            }
            catch { /* ignore cleanup issues */ }
        }
    }
}