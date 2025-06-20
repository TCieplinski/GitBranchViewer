using GitBranchViewer.Core.Services;
using System.CommandLine;

namespace GitBranchViewer.Cli.Commands
{
    public static class CompareCommand
    {
        public static Command Create(BranchComparisonService service)
        {
            var command = new Command("compare", "Compare two Git branches using merge repo");

            var sourceArg = new Argument<string>("source", "Source branch name");
            var targetArg = new Argument<string>("target", "Target branch name");
            var remoteArg = new Argument<string>("remote", "Remote Git URL");

            command.AddArgument(sourceArg);
            command.AddArgument(targetArg);
            command.AddArgument(remoteArg);

            command.SetHandler((source, target, remote) =>
            {
                var result = service.CompareViaMergeRepo(remote, source, target);
                Console.WriteLine(result);
            }, sourceArg, targetArg, remoteArg);

            return command;
        }
    }
}