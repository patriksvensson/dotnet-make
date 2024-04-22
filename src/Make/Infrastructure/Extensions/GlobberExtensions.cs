using Spectre.IO;

namespace Make;

public static class GlobberExtensions
{
    public static IEnumerable<FilePath> GetFiles(
        this IGlobber globber,
        string pattern,
        GlobberSettings settings)
    {
        return globber
            .Match(pattern, settings)
            .OfType<FilePath>();
    }
}