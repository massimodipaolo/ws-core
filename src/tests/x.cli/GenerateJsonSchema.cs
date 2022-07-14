using Spectre.Console;

namespace x.cli;

public class GenerateJsonSchema
{
    [Fact]
    public async Task Check_generationByPathAndAssembly()
    {
        // arrange
        var args = new string[] { "jschema", "--path", "C:\\Projects\\ws-core\\src\\modules", "--assembly", "Diagnostic,Gateway" };
        AnsiConsole.Record();

        // act
        var result = await ws.core.cli.Program.Main(args);

        // assert
        Assert.Equal(0, result);
        var text = AnsiConsole.ExportText();
        /*
💤 Module on C:\Projects\ws-core\src\modules
Generating schema for Diagnostic
👌 Done!
────────────────────────────────────────────────────────────────────────────────
Generating schema for Gateway
👌 Done!         
         */
        Assert.Contains($"Module on {args[2]}", text);
        Assert.True(args[4].Split(',').All(_ => text.Contains($"Generating schema for {_}")));
        Assert.True(args[4].Split(',').Count() == text.Split("👌 Done!").Length - 1);
    }
}