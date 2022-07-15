using Spectre.Console;
using Spectre.Console.Cli;
using System.Reflection;
using ws.core.cli.Command;


namespace ws.core.cli;

public partial class Program {
    protected Program() { }
    public static async Task<int> Main(string[] args)
    {
        var app = new CommandApp();
        app.SetDefaultCommand<GenerateJsonSchema>();

        app.Configure(config =>
        {
            var name = Assembly.GetEntryAssembly()?.GetName()?.Name ?? "";
            AnsiConsole.Write(
                new FigletText(name)
                    .LeftAligned()
                    .Color(Color.Silver));

            config
            .CaseSensitivity(CaseSensitivity.None)
            .SetApplicationName(name)
            .ValidateExamples();

            config.AddCommand<GenerateJsonSchema>("jschema")
                .WithDescription(":bookmark_tabs: Generate modules [hotpink3]json-schema.json[/]")
                .WithExample(new[] { "jschema", "--path", "C:\\Projects\\ws-core\\src\\modules", "--assembly", "Diagnostic,Gateway" });
        });
        return await app.RunAsync(args).ConfigureAwait(false);
    }
}