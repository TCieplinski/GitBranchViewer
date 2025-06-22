using System;
using System.Diagnostics;
using System.IO;

namespace GitBranchViewer.Core.Services
{
    public class QuickCompareService
    {
        private readonly string _diffPath;

        public QuickCompareService(string diffToolPath)
        {
            _diffPath = diffToolPath;
        }
        public string CompareDirectories(string pathA, string pathB)
        {

            if (!File.Exists(_diffPath))
                throw new FileNotFoundException("diff.exe not found at configured path: " + _diffPath);

            var psi = new ProcessStartInfo
            {
                FileName = @"C:\Program Files\Git\usr\bin\diff.exe",
                Arguments = $"-ru \"{pathA}\" \"{pathB}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            if (!File.Exists(psi.FileName))
                throw new FileNotFoundException($"Cannot find: {psi.FileName}");

            using var proc = Process.Start(psi);
            string output = proc.StandardOutput.ReadToEnd();
            string error = proc.StandardError.ReadToEnd();
            proc.WaitForExit();

            if (proc.ExitCode != 0 && string.IsNullOrWhiteSpace(output))
                throw new InvalidOperationException($"diff failed: {error}");

            return output;
        }
    }
}