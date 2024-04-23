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
}