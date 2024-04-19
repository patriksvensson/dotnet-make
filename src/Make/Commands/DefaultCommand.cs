using System.ComponentModel;
using Spectre.IO;

namespace Make;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class DefaultCommand : AsyncCommand<DefaultCommand.Settings>
{
    private readonly RootFinder _rootFinder;
    private readonly BuildRunners _runners;

    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "[TARGET]")]
        [Description("The target to run")]
        public string? Target { get; set; }

        [CommandOption("--trace")]
        [Description("Outputs trace logging for the make tool")]
        public bool Trace { get; set; }
    }

    public DefaultCommand(
        RootFinder rootFinder,
        BuildRunners runners)
    {
        _rootFinder = rootFinder ?? throw new ArgumentNullException(nameof(rootFinder));
        _runners = runners ?? throw new ArgumentNullException(nameof(runners));
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        // Create the build context
        var root = _rootFinder.Find();
        var buildContext = new BuildContext(
            root,
            settings.Target,
            settings.Trace,
            context.Remaining);

        // Figure out which build tool to invoke
        var runner = _runners.GetBuildRunner(buildContext);
        if (runner == null)
        {
            throw new MakeException("Could not find a suitable build tool", null);
        }

        // Invoke it and return the result
        return await runner.Run(buildContext);
    }
}