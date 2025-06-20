using GitBranchViewer.Core.Config;
using GitBranchViewer.Core.Services;
using System;

class Program
{
    static void Main(string[] args)
    {
        var settings = ConfigLoader.Load();
        if (settings.GitRepositories.Count == 0)
        {
            Console.WriteLine("No repositories configured.");
            return;
        }

        // Pick repository
        Console.WriteLine("Available repositories:");
        for (int i = 0; i < settings.GitRepositories.Count; i++)
            Console.WriteLine($"{i + 1}. {settings.GitRepositories[i].Name}");

        Console.Write("Select repository [1-N]: ");
        if (!int.TryParse(Console.ReadLine(), out int repoChoice) || repoChoice < 1 || repoChoice > settings.GitRepositories.Count)
        {
            Console.WriteLine("Invalid choice.");
            return;
        }

        var repo = settings.GitRepositories[repoChoice - 1];

        Console.Write("Use merge repo mode? (y/N): ");
        var useMerge = Console.ReadLine()?.Trim().ToLowerInvariant() == "y";

        if (useMerge)
        {
            var mergeManager = new MergeRepoManager(settings);
            var comparer = new BranchComparisonService(new GitService(repo.Path), settings.DefaultDiffOptions, mergeManager);

            Console.Write("Enter branch A name: ");
            string branchA = Console.ReadLine()?.Trim() ?? "";

            Console.Write("Enter branch B name: ");
            string branchB = Console.ReadLine()?.Trim() ?? "";

            Console.WriteLine($"\n🔍 Running merge-mode diff between {branchA} and {branchB}...");
            var diff = comparer.CompareViaMergeRepo(repo.RemoteUrl, branchA, branchB);
            Console.WriteLine(diff);
        }
        else
        {
            var git = new GitService(repo.Path);
            var branches = git.GetBranches();

            Console.WriteLine("\nAvailable branches:");
            for (int i = 0; i < branches.Count; i++)
                Console.WriteLine($"{i + 1}. {branches[i]}");

            Console.Write("Select branch A: ");
            var branchA = branches[int.Parse(Console.ReadLine()) - 1];

            Console.Write("Select branch B: ");
            var branchB = branches[int.Parse(Console.ReadLine()) - 1];

            var comparer = new BranchComparisonService(git, settings.DefaultDiffOptions);

            Console.WriteLine($"\n🔍 Comparing files between '{branchA}' and '{branchB}':");
            var files = comparer.GetChangedFiles(branchA, branchB);
            foreach (var file in files)
                Console.WriteLine("  " + file);

            Console.WriteLine($"\n📜 Commits in '{branchB}' not in '{branchA}':");
            var commits = comparer.GetCommitDifferences(branchA, branchB);
            foreach (var c in commits)
                Console.WriteLine($"  {c.Hash} | {c.Author} | {c.Date} | {c.Message}");
        }
    }
}