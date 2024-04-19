using Make.Extensions;

namespace Make;

public sealed class BuildRunners
{
    private readonly IReadOnlyList<IBuildRunner> _runners;

    public BuildRunners(IEnumerable<IBuildRunner> runners)
    {
        _runners = runners.ToSafeReadOnlyList();
    }

    public IBuildRunner? GetBuildRunner(BuildContext context)
    {
        foreach (var runner in _runners)
        {
            if (runner.CanHandle(context))
            {
                return runner;
            }
        }

        return null;
    }
}