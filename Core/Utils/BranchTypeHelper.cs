using System;
using System.Collections.Generic;
using System.Linq;

namespace GitBranchViewer.Core.Utils
{
    public static class BranchTypeHelper
    {
        private static readonly Dictionary<string, string> PrefixMap = new(StringComparer.OrdinalIgnoreCase)
        {
            { "main", "main" },
            { "master", "main" },
            { "develop", "develop" },
            { "dev", "develop" },
            { "feature/", "feature" },
            { "feat/", "feature" },
            { "bugfix/", "bugfix" },
            { "fix/", "bugfix" },
            { "hotfix/", "hotfix" },
            { "release/", "release" },
            { "support/", "support" },
            { "test/", "test" },
            { "experimental/", "experimental" }
        };

        /// <summary>
        /// Attempts to classify a branch into a semantic type such as "feature", "bugfix", etc.
        /// </summary>
        public static string GetBranchTypeFromName(string branchName)
        {
            foreach (var pair in PrefixMap)
            {
                if (branchName.StartsWith(pair.Key, StringComparison.OrdinalIgnoreCase))
                    return pair.Value;
            }

            // Fallback: extract first segment or return "unknown"
            var fallback = branchName.Split(new[] { '/', '-' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            return string.IsNullOrEmpty(fallback) ? "unknown" : fallback.ToLowerInvariant();
        }
    }
}