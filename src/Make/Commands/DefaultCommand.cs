using System.ComponentModel;
using Spectre.IO;

namespace Make;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class DefaultCommand : AsyncCommand<DefaultCommand.Settings>
{
    private readonly BuildRunnerSelector _buildRunnerSelector;
    private readonly BuildRunners _runners;

    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "[TARGET]")]
        [Description("The target to run")]
        public string? Target { get; set; }

        [CommandOption("--trace", IsHidden = true)]
        [Description("Outputs trace logging for the make tool")]
        public bool Trace { get; set; }
    }

    public DefaultCommand(
        BuildRunnerSelector buildRunnerSelector,
        BuildRunners runners)
    {
        _buildRunnerSelector = buildRunnerSelector ?? throw new ArgumentNullException(nameof(buildRunnerSelector));
        _runners = runners ?? throw new ArgumentNullException(nameof(runners));
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        // Create the build context
        var result = _buildRunnerSelector.Find(settings.Trace);
        if (result == null)
        {
            throw new MakeException("Could not find a suitable build tool", null);
        }

        var root = result.Value.Root;
        var runner = result.Value.Runner;

        var buildContext = new BuildContext(
            root,
            settings.Target,
            settings.Trace,
            context.Remaining);

        return await runner.Run(buildContext);
    }
}