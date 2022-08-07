using System.Text.Json;
using Spectre.Console;

namespace SnooSharp.Example;

public static class OptionsRetrieval
{
    private const string Filename = "opts.json";
    
    public static async Task<SnooClientOptions> GetOptions()
    {
        SnooClientOptions? opts;
        if (File.Exists(Filename) && AnsiConsole.Confirm($"Load saved details?"))
        {
            var contents = await File.ReadAllTextAsync(Filename);
            opts = JsonSerializer.Deserialize<SnooClientOptions>(contents);
            if (opts != default)
            {
                return opts;
            }
            AnsiConsole.Markup("[red]There was an issue loading the details, please re-enter.[/]");
        }
        var name = AnsiConsole.Ask<string>("[green]name[/]:");
        var password = AnsiConsole.Ask<string>("[green]password[/]:");
        opts = new SnooClientOptions()
        {
            Username = name,
            Password = password,
        };

        if (AnsiConsole.Confirm("Save these details?"))
        {
            var content = JsonSerializer.Serialize(opts);
            await File.WriteAllTextAsync(Filename, content);
        }

        return opts;
    }
}