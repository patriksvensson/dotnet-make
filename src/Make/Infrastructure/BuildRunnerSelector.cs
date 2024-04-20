using Spectre.IO;

namespace Make;

public sealed class BuildRunnerSelector
{
    private readonly IGlobber _globber;
    private readonly IEnvironment _environment;
    private readonly IAnsiConsole _console;
    private readonly BuildRunners _runners;

    public BuildRunnerSelector(
        IGlobber globber,
        IEnvironment environment,
        IAnsiConsole console,
        BuildRunners runners)
    {
        _globber = globber ?? throw new ArgumentNullException(nameof(globber));
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        _console = console ?? throw new ArgumentNullException(nameof(console));
        _runners = runners ?? throw new ArgumentNullException(nameof(runners));
    }

    public (DirectoryPath Root, IBuildRunner Runner)? Find(bool trace)
    {
        var comparer = new PathComparer(isCaseSensitive: false);

        foreach (var runner in _runners.GetBuildRunners())
        {
            var current = _environment.WorkingDirectory;
            while (current is { IsRoot: false })
            {
                foreach (var glob in runner.GetGlobs())
                {
                    var candidates = _globber
                        .Match(glob, new GlobberSettings
                        {
                            Root = current,
                            Comparer = comparer,
                        });

                    if (candidates.Any())
                    {
                        if (trace)
                        {
                            _console.MarkupLine($"[gray]Found root:[/] {current.FullPath} [gray]using glob[/] {glob}");
                        }

                        return (current, runner);
                    }
                }

                // Check the parent directory
                current = current.GetParent();
            }
        }

        return null;
    }
}