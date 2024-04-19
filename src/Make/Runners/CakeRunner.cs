using Spectre.IO;

namespace Make;

public sealed class CakeRunner : IBuildRunner
{
    private readonly IFileSystem _fileSystem;
    private readonly IEnvironment _environment;
    private readonly IProcessRunner _processRunner;

    public CakeRunner(
        IFileSystem fileSystem,
        IEnvironment environment,
        IProcessRunner processRunner)
    {
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        _processRunner = processRunner ?? throw new ArgumentNullException(nameof(processRunner));
    }

    public bool CanHandle(BuildContext context)
    {
        var root = context.Root ?? _environment.WorkingDirectory;
        var buildFile = root.CombineWithFilePath("build.cake");
        return _fileSystem.Exist(buildFile);
    }

    public async Task<int> Run(BuildContext context)
    {
        var root = context.Root ?? _environment.WorkingDirectory;

        var args = string.Join(" ", context.RemainingArguments.Raw);
        if (context.Target != null)
        {
            args += $"--target {context.Target}";
        }

        return await _processRunner.Run(
            "dotnet", args: "cake " + args,
            workingDirectory: root);
    }
}