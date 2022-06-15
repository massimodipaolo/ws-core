﻿using Newtonsoft.Json.Schema.Generation;
using Newtonsoft.Json.Serialization;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace ws.core.cli.Command;

public class GenerateJsonSchema : Command<GenerateJsonSchema.Settings>
{
    public class Settings : CommandSettings
    {
    }


    public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
    {
        var location = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        var module_dir = System.IO.Path.Combine(location ?? "", "..\\..\\..\\..\\..\\modules");
        AnsiConsole.MarkupLine($":zzz: Module on [blue]{module_dir}[/]");
        var extensions = System.IO.Directory.GetDirectories(module_dir).Select(_ => new System.IO.DirectoryInfo(_));
        var maxAllowed = 10;
        var extensionSelected
            = AnsiConsole.Prompt(
            new MultiSelectionPrompt<string>()
                .Title($"Select one or more [green]modules[/]{Environment.NewLine}Max items allowed [hotpink3]{maxAllowed}[/]")
                .Required()
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more modules)[/]")
                .InstructionsText(
                    "[grey](Press [blue]<space>[/] to toggle a module, " +
                    "[green]<enter>[/] to accept)[/]")
                .AddChoices<string>(extensions.Select(_ => _.Name)));

        foreach (var extension in extensions
            .Where(_ => extensionSelected.Contains(_.Name))
            .Take(maxAllowed))
        {
            var extensionName = extension.Name;
            var assemblyFile = System.IO.Path.Combine(extension.FullName, $"bin\\Debug\\net6.0\\Ws.Core.Extensions.{extensionName}.dll");

            if (File.Exists(assemblyFile))
            {
                try
                {
                    AnsiConsole.MarkupLine($"Generating schema for [blue]{extensionName}[/]");
                    var result = GenerateOptionsJsonSchema(assemblyFile, extension.FullName);
                    AnsiConsole.MarkupLine(
                        result ? 
                        $":ok_hand: [green]Done![/]" : 
                        $":angry_face: :broken_heart: [red]Generation skipped for {assemblyFile}[/]");
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($":angry_face: :broken_heart: [red]{ex}[/]");
                }
            }
            else
                AnsiConsole.MarkupLine($":angry_face: Assembly [red]{extensionName}[/] not found");

            AnsiConsole.Write(new Rule());
        }

        return 0;
    }

    public static bool GenerateOptionsJsonSchema(string assemblyFile, string extensionPath)
    {
        var extOptions = Assembly.Load(File.ReadAllBytes(assemblyFile))?.ExportedTypes?.FirstOrDefault(t => t != null && typeof(Ws.Core.Extensions.Base.IOptions).IsAssignableFrom(t) && !t.IsInterface);
        if (extOptions != null)
        {
            var schemaGenerator = new JSchemaGenerator
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            schemaGenerator.GenerationProviders
                .Add(new Newtonsoft.Json.Schema.Generation.StringEnumGenerationProvider());
            schemaGenerator.DefaultRequired = Newtonsoft.Json.Required.Default;
            schemaGenerator.SchemaReferenceHandling = Newtonsoft.Json.Schema.Generation.SchemaReferenceHandling.All;

            var schema = schemaGenerator.Generate(extOptions, rootSchemaNullable: true).ToString();
            System.IO.File.WriteAllText(System.IO.Path.Combine(extensionPath, "json-schema.json"), schema);
            return true;
        }
        return false;
    }
}
