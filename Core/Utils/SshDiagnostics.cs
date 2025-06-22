using System;
using System.Diagnostics;
using System.Threading;

namespace GitBranchViewer.Core.Utils
{
    public static class SshDiagnostics
    {
        public static string RunAll(string sshHost = "git@gitlab.com")
        {
            var report = new System.Text.StringBuilder();
            report.AppendLine("🔍 Running SSH diagnostics...\n");

            report.AppendLine("[1] Check SSH Installed:");
            report.AppendLine(CheckSshInstalled());

            report.AppendLine("\n[2] Check SSH Agent:");
            report.AppendLine(CheckSshAgentStatus());

            report.AppendLine("\n[3] List Loaded Keys:");
            report.AppendLine(ListLoadedKeys());

            report.AppendLine("\n[4] Test SSH Connection:");
            report.AppendLine(TestSshConnection(sshHost));

            report.AppendLine("\n✅ SSH diagnostics complete.");
            return report.ToString();
        }

        public static string CheckSshInstalled()
        {
            try
            {
                var output = Run("ssh", "-V", captureStdErr: true);
                return "✔ SSH installed: " + output.Trim();
            }
            catch (Exception ex)
            {
                return "❌ SSH not found: " + ex.Message;
            }
        }

        public static string CheckSshAgentStatus()
        {
            var agentEnv = Environment.GetEnvironmentVariable("SSH_AUTH_SOCK");
            return !string.IsNullOrEmpty(agentEnv)
                ? "✔ SSH agent detected: " + agentEnv
                : "⚠ SSH agent not detected (SSH_AUTH_SOCK not set)";
        }

        public static string ListLoadedKeys()
        {
            try
            {
                var output = Run("ssh-add", "-l");
                if (output.Contains("The agent has no identities") || output.Contains("Could not open a connection"))
                    return "⚠ No SSH identities loaded.";
                else
                    return "✔ SSH keys loaded:\n" + output;
            }
            catch (Exception ex)
            {
                return "❌ ssh-add -l failed: " + ex.Message;
            }
        }

        public static string TestSshConnection(string sshHost)
        {
            try
            {
                var output = Run("ssh", $"-T {sshHost} -o BatchMode=yes", captureStdErr: true);
                return "✔ SSH connection test passed:\n" + output;
            }
            catch (Exception ex)
            {
                return "❌ SSH connection failed:\n" + ex.Message;
            }
        }

        private static string Run(string file, string args, bool captureStdErr = false)
        {
            int timeoutMs = 15000;

            var psi = new ProcessStartInfo
            {
                FileName = file,
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = captureStdErr,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var proc = Process.Start(psi);
            var output = captureStdErr ? proc.StandardError.ReadToEnd() : proc.StandardOutput.ReadToEnd();
            proc.WaitForExit(timeoutMs);
            if (!proc.HasExited)
            {
                proc.Kill();
                throw new TimeoutException("SSH test timed out.");
            }

            if (proc.ExitCode != 0)
                throw new InvalidOperationException($"{file} exited with code {proc.ExitCode}\n{output}");

            return output;
        }
    }
}