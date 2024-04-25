using Spectre.IO;

namespace Make;

public sealed class CakeRunner : IBuildRunner
{
    private readonly IProcessRunner _processRunner;

    public string Name { get; } = "Cake Runner";
    public int Order { get; } = 0;

    public CakeRunner(IProcessRunner processRunner)
    {
        _processRunner = processRunner ?? throw new ArgumentNullException(nameof(processRunner));
    }

    public IEnumerable<string> GetKeywords()
    {
        return ["cake"];
    }

    public IEnumerable<string> GetGlobs(MakeSettings settings)
    {
        return ["./build.cake"];
    }

    public bool CanRun(MakeSettings settings, DirectoryPath path)
    {
        // Since we only match on a single thing (with no wildcards),
        // we're sure that we can run it.
        return true;
    }

    public async Task<int> Run(BuildContext context)
    {
        var args = GetArgs(context);

        return await _processRunner.Run(
            "dotnet", args: "cake " + args,
            trace: context.Trace,
            workingDirectory: context.Root);
    }

    private static string GetArgs(BuildContext context)
    {
        var args = new List<string>();

        if (context.Target != null)
        {
            args.Add("--target");
            args.Add($"\"{context.Target}\"");
        }

        context.AddArgs(args);

        return string.Join(" ", args);
    }
}