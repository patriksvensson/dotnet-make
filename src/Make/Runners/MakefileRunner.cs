namespace Make;

public sealed class MakefileRunner : IBuildRunner
{
    private readonly IProcessRunner _processRunner;

    public MakefileRunner(IProcessRunner processRunner)
    {
        _processRunner = processRunner ?? throw new ArgumentNullException(nameof(processRunner));
    }

    public IEnumerable<string> GetGlobs()
    {
        return ["./Makefile"];
    }

    public async Task<int> Run(BuildContext context)
    {
        return await _processRunner.Run(
            "make", context.Target,
            trace: context.Trace,
            workingDirectory: context.Root);
    }
}