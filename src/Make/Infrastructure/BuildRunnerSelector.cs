using Spectre.IO;

namespace Make;

public sealed class BuildRunnerSelector
{
    private readonly IGlobber _globber;
    private readonly IEnvironment _environment;
    private readonly IAnsiConsole _console;
    private readonly BuildRunners _runners;
    private readonly Dictionary<string, IBuildRunner> _runnerLookup;

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

        _runnerLookup = new Dictionary<string, IBuildRunner>();
        foreach (var runner in _runners.GetBuildRunners())
        {
            foreach (var name in runner.GetKeywords())
            {
                _runnerLookup[name] = runner;
            }
        }
    }

    public (DirectoryPath Root, IBuildRunner Runner)? Find(MakeSettings settings)
    {
        var comparer = new PathComparer(isCaseSensitive: false);

        var current = _environment.WorkingDirectory;
        while (current is { IsRoot: false })
        {
            foreach (var runner in _runners.GetBuildRunners())
            {
                var names = new HashSet<string>(runner.GetKeywords());
                if (settings.Prefer != null)
                {
                    if (!names.Contains(settings.Prefer))
                    {
                        continue;
                    }
                }

                foreach (var glob in runner.GetGlobs(settings))
                {
                    var candidates = _globber
                        .Match(glob, new GlobberSettings
                        {
                            Root = current,
                            Comparer = comparer,
                        });

                    if (candidates.Any())
                    {
                        if (settings.Trace)
                        {
                            _console.MarkupLine(
                                $"[gray]Found root[/] {current.FullPath} [gray]using glob[/] {glob}");
                        }

                        if (runner.CanRun(settings, current))
                        {
                            _console.MarkupLine($"[gray]Using runner[/] {runner.Name}");
                            return (current, runner);
                        }
                    }
                }
            }

            // Check the parent directory
            current = current.GetParent();
        }

        return null;
    }
}