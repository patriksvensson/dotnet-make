using Spectre.IO;

namespace Make;

public sealed class ProjectRunner : IBuildRunner
{
    private readonly IGlobber _globber;
    private readonly IProcessRunner _processRunner;
    private readonly IAnsiConsole _console;

    public string Name { get; } = "Project Builder";
    public int Order { get; } = 4;

    private static readonly string _glob = "./*.{fsproj,csproj,vbproj}";

    public ProjectRunner(
        IGlobber globber,
        IProcessRunner processRunner,
        IAnsiConsole console)
    {
        _globber = globber ?? throw new ArgumentNullException(nameof(globber));
        _processRunner = processRunner ?? throw new ArgumentNullException(nameof(processRunner));
        _console = console ?? throw new ArgumentNullException(nameof(console));
    }

    public IEnumerable<string> GetKeywords()
    {
        return ["csproj", "fsproj", "vbproj", "project"];
    }

    public IEnumerable<string> GetGlobs(MakeSettings settings)
    {
        return new[] { _glob };
    }

    public bool CanRun(MakeSettings settings, DirectoryPath path)
    {
        var results = GetCandidates(path);
        if (results.Count > 1 && settings.Trace)
        {
            _console.MarkupLine("[gray]Project runner:[/] Found too many candidates");
        }

        return results.Count == 1;
    }

    public async Task<int> Run(BuildContext context)
    {
        var result = GetCandidates(context.Root).First();

        var args = new List<string>();
        args.Add("build");
        args.Add(result.GetFilename().FullPath);
        args.AddRange(context.GetArgs());

        return await _processRunner.Run(
            "dotnet", string.Join(" ", args),
            trace: context.Trace,
            workingDirectory: context.Root);
    }

    private List<FilePath> GetCandidates(DirectoryPath path)
    {
        return _globber.GetFiles(_glob, new GlobberSettings
        {
            Comparer = new PathComparer(isCaseSensitive: false),
            Root = path,
        }).ToList();
    }
}