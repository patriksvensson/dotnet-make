using Spectre.IO;

namespace Make;

public sealed class BuildContext
{
    public DirectoryPath? Root { get; }
    public IRemainingArguments RemainingArguments { get; }
    public string? Target { get; }

    public BuildContext(DirectoryPath? root, string? target, IRemainingArguments remainingArguments)
    {
        Root = root;
        Target = target;
        RemainingArguments = remainingArguments ?? throw new ArgumentNullException(nameof(remainingArguments));
    }
}