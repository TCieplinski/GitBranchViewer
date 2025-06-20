using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitBranchViewer.Core.Utils
{
    public static class FileHelper
    {
        public static string ExpandHomePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return path;

            if (path.StartsWith("~"))
            {
                var home = Environment.OSVersion.Platform switch
                {
                    PlatformID.Unix => Environment.GetEnvironmentVariable("HOME"),
                    PlatformID.MacOSX => Environment.GetEnvironmentVariable("HOME"),
                    _ => Environment.ExpandEnvironmentVariables("%USERPROFILE%")
                };

                return Path.Combine(home ?? "", path.Substring(1).TrimStart(Path.DirectorySeparatorChar));
            }

            return path;
        }
    }
}
