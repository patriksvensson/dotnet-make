using Microsoft.Extensions.DependencyInjection;
using Spectre.IO;
using Environment = Spectre.IO.Environment;

namespace Make;

public static class Program
{
    public static int Main(string[] args)
    {
        var app = new CommandApp<DefaultCommand>(BuildRegistrar());
        app.Configure(config =>
        {
            config.SetApplicationName("dotnet make");
            config.SetExceptionHandler(ex =>
            {
                if (ex is CommandRuntimeException)
                {
                    AnsiConsole.MarkupLine("[red]Error:[/]");
                    AnsiConsole.WriteLine(ex.Message);
                    AnsiConsole.WriteLine();
                    AnsiConsole.MarkupLine("Run [yellow]--help[/] to see available options");
                }
                else if (ex is MakeException makeEx)
                {
                    if (makeEx.Pretty != null)
                    {
                        AnsiConsole.Write(makeEx.Pretty);
                    }
                    else
                    {
                        AnsiConsole.MarkupLine($"[red]Error:[/] {ex.Message.RemoveMarkup()}");
                    }
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]Error:[/]");
                    AnsiConsole.WriteException(ex);
                }
            });
        });

        return app.Run(args);
    }

    private static ITypeRegistrar BuildRegistrar()
    {
        var services = new ServiceCollection();

        // Infrastructure
        services.AddSingleton<BuildRunnerSelector>();
        services.AddSingleton<BuildRunners>();
        services.AddSingleton<IProcessRunner, ProcessRunner>();

        // Runners
        services.AddSingleton<IBuildRunner, CakeRunner>();
        services.AddSingleton<IBuildRunner, MakefileRunner>();

        // File system
        services.AddSingleton<IFileSystem, FileSystem>();
        services.AddSingleton<IEnvironment, Environment>();
        services.AddSingleton<IPlatform, Platform>();
        services.AddSingleton<IGlobber, Globber>();

        return new TypeRegistrar(services);
    }
}