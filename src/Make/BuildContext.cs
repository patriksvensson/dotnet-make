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

    public IReadOnlyList<string> GetArgs()
    {
        var args = new List<string>();
        AddArgs(args);
        return args;
    }

    public void AddArgs(List<string> args)
    {
        foreach (var group in RemainingArguments.Parsed)
        {
            var name = group.Key;

            var values = group.ToArray();
            if (values.Length > 0)
            {
                foreach (var value in group)
                {
                    args.Add(name);

                    if (value != null)
                    {
                        args.Add(value);
                    }
                }
            }
            else
            {
                args.Add(name);
            }
        }

        args.AddRange(RemainingArguments.Raw);
    }
}