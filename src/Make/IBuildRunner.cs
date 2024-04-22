using Spectre.IO;

namespace Make;

public interface IBuildRunner
{
    string Name { get; }
    int Order { get; }

    IEnumerable<string> GetKeywords();
    IEnumerable<string> GetGlobs(MakeSettings settings);
    bool CanRun(MakeSettings settings, DirectoryPath path);
    Task<int> Run(BuildContext context);
}