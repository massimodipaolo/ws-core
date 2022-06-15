using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using Newtonsoft.Json.Schema.Infrastructure;
using Newtonsoft.Json.Serialization;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Reflection;
using ws.core.cli.Command;

var app = new CommandApp();
app.Configure(config =>
{
    AnsiConsole.Write(
        new FigletText(Assembly.GetEntryAssembly()?.GetName()?.Name ?? "")
            .LeftAligned()
            .Color(Color.Silver));

    config.AddCommand<GenerateJsonSchema>("jschema")
        //.WithAlias("hola")
        .WithDescription(":bookmark_tabs: Generate modules [hotpink3]json-schema.json[/]")
        .WithExample(new[] { "jschema" });
});
return app.Run(args);