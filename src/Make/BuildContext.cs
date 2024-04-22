using Spectre.IO;

namespace Make;

public sealed class BuildContext
{
    public DirectoryPath Root { get; }
    public IRemainingArguments RemainingArguments { get; }
    public string? Target { get; }
    public bool Trace { get; }

    public BuildContext(
        DirectoryPath root,
        string? target,
        bool trace,
        IRemainingArguments remainingArguments)
    {
        Root = root;
        Target = target;
        Trace = trace;
        RemainingArguments = remainingArguments ?? throw new ArgumentNullException(nameof(remainingArguments));
    }

    public List<string> GetArgs()
    {
        var result = new List<string>();

        foreach (var variable in RemainingArguments.Parsed)
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

        if (RemainingArguments.Raw.Count > 0)
        {
            result.AddRange(RemainingArguments.Raw);
        }

        return result;
    }
}