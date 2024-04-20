using Make.Extensions;

namespace Make;

public sealed class BuildRunners
{
    private readonly IReadOnlyList<IBuildRunner> _runners;

    public BuildRunners(IEnumerable<IBuildRunner> runners)
    {
        _runners = runners.ToSafeReadOnlyList();
    }

    public IEnumerable<IBuildRunner> GetBuildRunners()
    {
        return _runners;
    }
}