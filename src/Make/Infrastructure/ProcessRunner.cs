using Spectre.IO;

namespace Make;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
internal sealed class ProcessRunner : IProcessRunner
{
    private readonly IEnvironment _environment;
    private readonly IAnsiConsole _console;

    public ProcessRunner(
        IEnvironment environment,
        IAnsiConsole console)
    {
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        _console = console ?? throw new ArgumentNullException(nameof(console));
    }

    public async Task<int> Run(
        string executable, string? args,
        bool trace = false,
        DirectoryPath? workingDirectory = null)
    {
        var working = workingDirectory == null
            ? _environment.WorkingDirectory
            : workingDirectory.MakeAbsolute(_environment);

        if (trace)
        {
            _console.MarkupLine($"[gray]Executing:[/] {executable} {args}".Trim());
        }

        var exitCode = 0;
        await SimpleExec.Command.RunAsync(
            name: executable,
            args: args ?? string.Empty,
            noEcho: true,
            createNoWindow: true,
            workingDirectory: working.FullPath,
            handleExitCode: (e) =>
            {
                exitCode = e;
                return true;
            });

        return exitCode;
    }
}

public interface IProcessRunner
{
    Task<int> Run(
        string executable, string? args,
        bool trace = false,
        DirectoryPath? workingDirectory = null);
}