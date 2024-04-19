using System.Diagnostics;
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
        var args = GetArgs(context);

        return await _processRunner.Run(
            "dotnet", args: "cake " + args,
            trace: context.Trace,
            workingDirectory: root);
    }

    private static string GetArgs(BuildContext context)
    {
        var result = new List<string>();

        if (context.Target != null)
        {
            result.Add("--target");
            result.Add($"\"{context.Target}\"");
        }

        foreach (var variable in context.RemainingArguments.Parsed)
        {
            var option = variable.Key.Length > 1
                ? $"--{variable.Key}"
                : $"-{variable.Key}";

            var values = variable.ToArray();
            if (values.Length > 0)
            {
                foreach (var value in variable)
                {
                    result.Add(option);

                    if (value != null)
                    {
                        result.Add($"\"{value}\"");
                    }
                }
            }
            else
            {
                result.Add(option);
            }
        }

        if (context.RemainingArguments.Raw.Count > 0)
        {
            result.Add("--");
            result.AddRange(context.RemainingArguments.Raw);
        }

        return string.Join(" ", result);
    }
}