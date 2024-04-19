using Spectre.IO;

namespace Make;

public sealed class RootFinder
{
    private readonly IGlobber _globber;
    private readonly IFileSystem _fileSystem;
    private readonly IEnvironment _environment;

    public RootFinder(IGlobber globber, IFileSystem fileSystem, IEnvironment environment)
    {
        _globber = globber ?? throw new ArgumentNullException(nameof(globber));
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));
    }

    public DirectoryPath? Find()
    {
        var comparer = new PathComparer(isCaseSensitive: false);
        var files = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "README.md",
            "CODE_OF_CONDUCT.md",
            "LICENSE.md",
            "dotnet-tools.json",
            "global.json",
            "build.cake",
            "cake.config",
            ".git",
            ".artifacts",
            "src",
        };

        var current = _environment.WorkingDirectory;
        while (current is { IsRoot: false })
        {
            var candidates = _globber
                .Match("./*", new GlobberSettings
                {
                    Root = current,
                })
                .ToHashSet(comparer);

            foreach (var path in candidates)
            {
                if (files.Contains(path.Segments.Last()))
                {
                    return current;
                }
            }

            // Check the parent directory
            current = current.GetParent();
        }

        return null;
    }
}