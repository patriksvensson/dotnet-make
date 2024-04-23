using Spectre.IO;

namespace Make;

public sealed class MakefileRunner : IBuildRunner
{
    private readonly IProcessRunner _processRunner;

    public string Name { get; } = "Make Runner";
    public int Order { get; } = 2;

    public MakefileRunner(IProcessRunner processRunner)
    {
        _processRunner = processRunner ?? throw new ArgumentNullException(nameof(processRunner));
    }

    public IEnumerable<string> GetKeywords()
    {
        return ["make", "makefile"];
    }

    public IEnumerable<string> GetGlobs(MakeSettings settings)
    {
        return ["./Makefile"];
    }

    public bool CanRun(MakeSettings settings, DirectoryPath path)
    {
        // Since we only match on a single thing (with no wildcards),
        // we're sure that we can run it.
        return true;
    }

    public async Task<int> Run(BuildContext context)
    {
        var args = new List<string>();
        if (context.Target != null)
        {
            args.Insert(0, context.Target);
        }

        args.AddRange(context.RemainingArguments.Raw);

        return await _processRunner.Run(
            "make", string.Join(" ", args),
            trace: context.Trace,
            workingDirectory: context.Root);
    }
}