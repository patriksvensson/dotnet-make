namespace Make;

public sealed class CakeRunner : IBuildRunner
{
    private readonly IProcessRunner _processRunner;

    public CakeRunner(IProcessRunner processRunner)
    {
        _processRunner = processRunner ?? throw new ArgumentNullException(nameof(processRunner));
    }

    public IEnumerable<string> GetGlobs()
    {
        return ["./build.cake"];
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
        var result = new List<string>();

        if (context.Target != null)
        {
            result.Add("--target");
            result.Add($"\"{context.Target}\"");
        }

        // TODO: Update Spectre.Console to include contextual information
        // about the remaining arguments.
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