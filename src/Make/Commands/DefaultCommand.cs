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

        [CommandOption("--prefer <RUNNER>")]
        [Description("Uses the preferred runner. Available runners are [blue]cake[/], [blue]frosting[/], [blue]project[/], [blue]sln[/], [blue]traversal[/]")]
        public string? Prefer { get; set; }
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
        var options = new MakeSettings
        {
            Trace = settings.Trace,
            Prefer = settings.Prefer,
        };

        var result = _buildRunnerSelector.Find(options);
        if (result == null)
        {
            throw new MakeException("Could not find a suitable build tool", null);
        }

        var buildContext = new BuildContext(
            result.Value.Root,
            settings.Target,
            settings.Trace,
            context.Remaining);

        return await result.Value.Runner
            .Run(buildContext);
    }
}