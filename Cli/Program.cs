using GitBranchViewer.Core.Config;
using GitBranchViewer.Core.Services;
using System;
using System.Linq;

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

        // Select repository
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

        // Select comparison mode
        Console.WriteLine("\nSelect comparison mode:");
        Console.WriteLine("1. Local diff");
        Console.WriteLine("2. Merge repo diff");
        Console.WriteLine("3. Remote-only shallow diff");

        Console.Write("Mode [1-3]: ");
        var mode = Console.ReadLine()?.Trim();

        if (mode == "3" || repo.IsMultiBranch)
        {
            var remoteService = new RemoteCompareService(settings.RemoteTempFolderRoot);

            Console.Write("Enter branch A name: ");
            var branchA = Console.ReadLine()?.Trim();

            Console.Write("Enter branch B name: ");
            var branchB = Console.ReadLine()?.Trim();

            var files = remoteService.CompareBranches(repo.RemoteUrl, branchA, branchB);
            if (files.Count == 0)
            {
                Console.WriteLine("✅ No file differences detected.");
                return;
            }

            Console.WriteLine("\n📄 Changed files:");
            for (int i = 0; i < files.Count; i++)
                Console.WriteLine($"{i + 1}. {files[i].Status}  {files[i].FileName}");

            Console.Write("\nEnter file number to view content diff: ");
            if (!int.TryParse(Console.ReadLine(), out int fileIndex) || fileIndex < 1 || fileIndex > files.Count)
            {
                Console.WriteLine("Invalid selection.");
                return;
            }

            var filePath = files[fileIndex - 1].FileName;
            Console.WriteLine($"\n🔍 Diff for file: {filePath}");

            var diff = remoteService.GetFileDiff(repo.RemoteUrl, branchA, branchB, filePath);
            Console.WriteLine(diff);
        }
        else if (mode == "2") // Merge repo diff
        {
            Console.Write("Enter branch A name: ");
            var branchA = Console.ReadLine()?.Trim() ?? "";

            Console.Write("Enter branch B name: ");
            var branchB = Console.ReadLine()?.Trim() ?? "";

            var mergeManager = new MergeRepoManager(settings, repo.Name);
            var git = GitServiceFactory.CreateForMergeRepo(mergeManager.GetMergeFolderPath(), repo.RemoteUrl);
            var comparer = new BranchComparisonService(git, settings.DefaultDiffOptions, mergeManager);

            Console.WriteLine($"\n🔍 Running merge-mode diff between {branchA} and {branchB}...");
            var diff = comparer.CompareViaMergeRepo(repo.RemoteUrl, branchA, branchB);
            Console.WriteLine(diff);
        }
        else if (mode == "1") // Local repo diff
        {
            var git = GitServiceFactory.CreateForLocal(repo.Path);
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
        else
        {
            Console.WriteLine("❌ Invalid mode selected.");
        }
    }
}