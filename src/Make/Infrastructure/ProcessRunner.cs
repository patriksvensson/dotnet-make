using Spectre.IO;

namespace Make;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
internal sealed class ProcessRunner : IProcessRunner
{
    private readonly IEnvironment _environment;

    public ProcessRunner(IEnvironment environment)
    {
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));
    }

    public async Task<int> Run(
        string executable, string? args,
        DirectoryPath? workingDirectory = null)
    {
        var working = workingDirectory == null
            ? _environment.WorkingDirectory
            : workingDirectory.MakeAbsolute(_environment);

        var exitCode = 0;
        await SimpleExec.Command.RunAsync(
            name: executable,
            args: args ?? string.Empty,
            noEcho: true,
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
        DirectoryPath? workingDirectory = null);
}