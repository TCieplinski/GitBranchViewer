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
    /// Provides core Git operations using git.exe (process-based).
    /// All commands are executed inside the target repository path.
    /// </summary>
    public class GitService
    {
        private readonly string _repositoryPath;

        public GitService(string repositoryPath)
        {
            var resolvedPath = FileHelper.ExpandHomePath(repositoryPath);
            if (!Directory.Exists(Path.Combine(resolvedPath, ".git")))
                throw new ArgumentException("Invalid Git repository path.");

            _repositoryPath = resolvedPath;
        }

        public List<string> GetBranches()
        {
            var output = RunGitCommand("branch --list", _repositoryPath);
            return output
                .Split('\n')
                .Select(line => line.Trim().TrimStart('*').Trim())
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .ToList();
        }

        public List<(string Hash, string Author, string Date, string Message)> GetCommits(string branch, int count = 50)
        {
            var format = "--pretty=format:\"%h|%an|%ad|%s\"";
            var output = RunGitCommand($"log {branch} -n {count} {format}", _repositoryPath);
            return output
                .Split('\n')
                .Select(line => line.Trim('\"'))
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line =>
                {
                    var parts = line.Split('|');
                    if (parts.Length < 4)
                        return (Hash: "[invalid]", Author: "", Date: "", Message: line);
                    return (parts[0], parts[1], parts[2], parts[3]);
                })
                .ToList();
        }

        public List<GitFileChange> GetChangedFilesBetweenBranches(string branchA, string branchB)
        {
            var output = RunGitCommand($"diff --name-status {branchA}..{branchB}", _repositoryPath);

            return output.Split('\n')
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line =>
                {
                    var parts = line.Trim().Split('\t', 2);
                    return new GitFileChange(parts[0], parts[1]);
                })
                .ToList();
        }

        public string GetFileDiff(string filePath, string branchA, string branchB)
        {
            return RunGitCommand($"diff {branchA}..{branchB} -- \"{filePath}\"", _repositoryPath);
        }

        // ----------------------------
        // Static Git Utilities for MergeRepoManager
        // ----------------------------

        public static void InitRepository(string path, string remoteUrl)
        {
            RunGitCommand("init", path);
            RunGitCommand($"remote add origin {remoteUrl}", path);
            RunGitCommand("fetch", path);
        }

        public static void AddWorktree(string repoPath, string worktreePath, string branch)
        {
            RunGitCommand($"worktree add \"{worktreePath}\" {branch}", repoPath);
        }

        private static string RunGitCommand(string arguments, string workingDirectory)
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
                throw new InvalidOperationException($"Git command failed: {error}");

            return output;
        }
    }
}