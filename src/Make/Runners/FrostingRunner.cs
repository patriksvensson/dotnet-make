using Spectre.IO;

namespace Make;

public sealed class FrostingRunner : IBuildRunner
{
    private readonly IProcessRunner _processRunner;

    public string Name { get; } = "Frosting Runner";
    public int Order { get; } = 1;

    public FrostingRunner(IProcessRunner processRunner)
    {
        _processRunner = processRunner ?? throw new ArgumentNullException(nameof(processRunner));
    }

    public IEnumerable<string> GetKeywords()
    {
        return ["frosting"];
    }

    public IEnumerable<string> GetGlobs(MakeSettings settings)
    {
        return new[] { "build/Build.csproj" };
    }

    public bool CanRun(MakeSettings settings, DirectoryPath path)
    {
        // Since we only match on a single thing (with no wildcards),
        // we're sure that we can run it.
        return true;
    }

    public async Task<int> Run(BuildContext context)
    {
        return await _processRunner.Run(
            "dotnet", args: GetArgs(context),
            trace: context.Trace,
            workingDirectory: context.Root);
    }

    private static string GetArgs(BuildContext context)
    {
        var result = new List<string>
        {
            "run",
            "--project",
            "./build/Build.csproj",
            "--",
        };

        if (context.Target != null)
        {
            result.Add("--target");
            result.Add($"\"{context.Target}\"");
        }

        result.AddRange(context.RemainingArguments.Raw);

        return string.Join(" ", result);
    }
}